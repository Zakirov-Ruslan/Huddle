using Huddle.EventBus.Abstractions;
using Huddle.Grpc;
using Huddle.Voice.WebApi.Contracts;
using Huddle.Voice.WebApi.IntegrationEvents;
using Huddle.Voice.WebApi.Services;
using Huddle.Voice.WebApi.Voice;
using Livekit.Server.Sdk.Dotnet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Security.Claims;

namespace Huddle.Voice.WebApi;

public class Program
{
    /// <summary>LiveKit posts its webhooks here, see https://docs.livekit.io/home/server/webhooks/.</summary>
    private const string WebhookPath = "/api/livekit/webhook";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddRabbitMqEventBus("eventbus");

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var LIVEKIT_URL = builder.Configuration.GetValue<string>("LIVEKIT_URL")
            ?? throw new InvalidOperationException("LIVEKIT_URL environment variable not defined");
        var LIVEKIT_API_KEY = builder.Configuration.GetValue<string>("LIVEKIT_API_KEY")
            ?? throw new InvalidOperationException("LIVEKIT_API_KEY environment variable not defined");
        var LIVEKIT_API_SECRET = builder.Configuration.GetValue<string>("LIVEKIT_API_SECRET")
            ?? throw new InvalidOperationException("LIVEKIT_API_SECRET environment variable not defined");

        var identityUrl = Environment.GetEnvironmentVariable("IDENTITY_URL")
            ?? throw new ArgumentNullException("IDENTITY_URL environment variable not defined");
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = identityUrl;
            options.Audience = "huddle.channel.api";
            options.RequireHttpsMetadata = true;

            options.TokenValidationParameters.ValidateAudience = true;
            options.TokenValidationParameters.ValidateLifetime = true;
            options.TokenValidationParameters.ValidateIssuer = true;
            options.TokenValidationParameters.ValidIssuer = identityUrl;
            options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(2);
        });
        builder.Services.AddAuthorization();

        // GRPC
        // https://learn.microsoft.com/ru-ru/aspnet/core/grpc/loadbalancing?view=aspnetcore-9.0 - Load balancing
        builder.Services.AddGrpcClient<ChannelAccess.ChannelAccessClient>(options =>
        {
            options.Address = new Uri(Environment.GetEnvironmentVariable("CHANNELS_URL")
                ?? throw new ArgumentNullException("CHANNELS_URL environment variable not defined"));
        });
        builder.Services.AddScoped<GrpcChannelAccessClient>();

        // Voice presence: LiveKit is the source of truth, Redis keeps a projection of it that is rebuilt
        // from LiveKit whenever the projection is gone (see RedisVoiceStateStore).
        var redisConnectionString = builder.Configuration.GetConnectionString("redis")
            ?? throw new InvalidOperationException("Redis connection string not defined");
        builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var options = ConfigurationOptions.Parse(redisConnectionString);

            // The cache must not take the voice service down: the same setting is used by the SignalR
            // backplane, and the projection is rebuilt from LiveKit as soon as Redis is back.
            options.AbortOnConnectFail = false;

            return ConnectionMultiplexer.Connect(options);
        });
        builder.Services.AddSingleton<IVoiceStateStore, RedisVoiceStateStore>();
        builder.Services.AddSingleton<IVoiceRoomDirectory, LiveKitVoiceRoomDirectory>();
        builder.Services.AddSingleton<ServerVoiceStateProvider>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        // LiveKit delivers webhooks over plain HTTP in the development environment and does not follow
        // redirects, so that single path must not be redirected to HTTPS.
        app.UseWhen(
            context => !context.Request.Path.StartsWithSegments(WebhookPath),
            branch => branch.UseHttpsRedirection());

        app.UseAuthentication();
        app.UseAuthorization();

        // Access Token generator
        app.MapGet(
            "api/livekit/token",
            async ([FromQuery] string serverId,
                   [FromQuery] string channelId,
                   HttpContext context,
                   GrpcChannelAccessClient grpcChannelAccessClient,
                   ILogger<Program> logger) =>
            {
                var user = context.User;
                if (user.Identity?.IsAuthenticated != true)
                    return Results.Unauthorized();

                var userId = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    return Results.BadRequest("User ID not found");

                logger.LogInformation("Token request by user: {userId} for channel: {channelId}", userId, channelId);

                try
                {
                    var canAccess = await grpcChannelAccessClient.CheckChannelAccessAsync(Guid.Parse(channelId), Guid.Parse(userId));
                    if (!canAccess)
                        return Results.Forbid();
                }
                catch (Exception ex)
                {
                    logger.LogError($"Exception on grpc access check: {ex.Message}");
                    return Results.Problem("Failed to validate permissions", statusCode: 500);
                }

                var roomName = VoiceRoomName.Format(Guid.Parse(serverId), Guid.Parse(channelId));
                var displayName = user.FindFirst("name")?.Value ?? userId;

                var token = new AccessToken(LIVEKIT_API_KEY, LIVEKIT_API_SECRET)
                    .WithIdentity(userId)
                    .WithName(displayName)
                    .WithGrants(new VideoGrants { RoomJoin = true, Room = roomName })
                    .WithTtl(TimeSpan.FromHours(1));

                logger.LogInformation("Token successfuly generated for user {userId} and channel {channelId}", userId, channelId);

                return Results.Ok(token.ToJwt());
            }
        )
        .RequireAuthorization();

        // Voice presence snapshot of a whole server, used when the server page is opened.
        // One request per server: the participants are grouped by channel on the server side, so the client
        // neither multiplies requests per channel nor repeats the access check for each of them.
        app.MapGet(
            "api/livekit/servers/{serverId:guid}/participants",
            async (Guid serverId,
                   HttpContext context,
                   GrpcChannelAccessClient grpcChannelAccessClient,
                   ServerVoiceStateProvider voiceStateProvider,
                   ILogger<Program> logger) =>
            {
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (context.User.Identity?.IsAuthenticated != true || !Guid.TryParse(userId, out var identityId))
                    return Results.Unauthorized();

                try
                {
                    var canAccess = await grpcChannelAccessClient.CheckServerAccessAsync(serverId, identityId);
                    if (!canAccess)
                        return Results.Forbid();
                }
                catch (Exception ex)
                {
                    logger.LogError($"Exception on grpc access check: {ex.Message}");
                    return Results.Problem("Failed to validate permissions", statusCode: 500);
                }

                var snapshot = await voiceStateProvider.GetAsync(serverId, context.RequestAborted);

                return Results.Ok(snapshot.ToDto(serverId));
            }
        )
        .RequireAuthorization();

        // Webhook handler
        // https://docs.livekit.io/home/server/webhooks/
        //
        // Deliberately without `RequireAuthorization()`: LiveKit authenticates its callbacks with a JWT
        // signed by the LiveKit API secret, not with a bearer token of a Huddle user, so the standard
        // JwtBearer scheme used by every other endpoint here would reject them. The request is instead
        // authenticated by WebhookReceiver.Receive, which verifies that JWT and the sha256 digest of the
        // body, so an unsigned request (or one with a modified body) is refused.
        var webhookReceiver = new WebhookReceiver(LIVEKIT_API_KEY, LIVEKIT_API_SECRET);
        app.MapPost(
            WebhookPath,
            async (HttpRequest request,
                   IEventBus eventBus,
                   IVoiceStateStore stateStore,
                   ILogger<Program> logger) =>
            {
                var authorization = request.Headers.Authorization.ToString();
                if (string.IsNullOrEmpty(authorization))
                    return Results.BadRequest("Authorization header is required");

                var body = new StreamReader(request.Body);
                string postData = await body.ReadToEndAsync();

                WebhookEvent webhookEvent;
                try
                {
                    webhookEvent = webhookReceiver.Receive(postData, authorization);
                }
                catch (Exception exception)
                {
                    logger.LogWarning(exception, "Rejected a LiveKit webhook: the signature could not be verified");
                    return Results.Unauthorized();
                }

                logger.LogInformation("Recieved webhookEvent: {@webhookEvent}", webhookEvent);

                if (!VoiceRoomName.TryParse(webhookEvent.Room?.Name, out var room))
                {
                    logger.LogWarning("Ignoring a LiveKit webhook for the unknown room {RoomName}", webhookEvent.Room?.Name);
                    return Results.Ok();
                }

                switch (webhookEvent.Event)
                {
                    case "participant_joined":
                        {
                            if (!TryReadUserId(webhookEvent, logger, out var userId))
                                return Results.Ok();

                            var participant = new VoiceParticipant(userId, DisplayNameOf(webhookEvent), JoinedAtOf(webhookEvent));
                            var version = await stateStore.AddParticipantAsync(room.ServerId, room.ChannelId, participant);

                            await eventBus.PublishAsync(new VoiceParticipantJoined(
                                serverId: room.ServerId,
                                channelId: room.ChannelId,
                                userId: participant.UserId,
                                displayName: participant.DisplayName,
                                joinedAt: participant.JoinedAt,
                                version: version));

                            return Results.Ok();
                        }
                    case "participant_left":
                    case "participant_connection_aborted":
                        {
                            if (!TryReadUserId(webhookEvent, logger, out var userId))
                                return Results.Ok();

                            var version = await stateStore.RemoveParticipantAsync(room.ServerId, room.ChannelId, userId);

                            await eventBus.PublishAsync(new VoiceParticipantLeft(
                                serverId: room.ServerId,
                                channelId: room.ChannelId,
                                userId: userId,
                                reason: webhookEvent.Event,
                                version: version));

                            return Results.Ok();
                        }
                    case "room_finished":
                        {
                            var version = await stateStore.RemoveChannelAsync(room.ServerId, room.ChannelId);

                            await eventBus.PublishAsync(new VoiceChannelClosed(
                                serverId: room.ServerId,
                                channelId: room.ChannelId,
                                version: version));

                            return Results.Ok();
                        }
                    case "room_started":
                    case "track_published":
                    case "track_unpublished":
                    case "egress_started":
                    case "egress_updated":
                    case "egress_ended":
                    case "ingress_started":
                    case "ingress_ended":
                    default:
                        return Results.Ok();
                }
            }
        );

        app.Run();
    }

    private static bool TryReadUserId(WebhookEvent webhookEvent, ILogger logger, out Guid userId)
    {
        if (Guid.TryParse(webhookEvent.Participant?.Identity, out userId))
            return true;

        logger.LogWarning(
            "Ignoring a LiveKit webhook {EventName} with the malformed participant identity {Identity}",
            webhookEvent.Event,
            webhookEvent.Participant?.Identity);

        return false;
    }

    private static string DisplayNameOf(WebhookEvent webhookEvent) =>
        string.IsNullOrWhiteSpace(webhookEvent.Participant?.Name)
            ? webhookEvent.Participant?.Identity ?? string.Empty
            : webhookEvent.Participant.Name;

    private static DateTimeOffset JoinedAtOf(WebhookEvent webhookEvent)
    {
        var joinedAt = webhookEvent.Participant?.JoinedAt ?? 0;

        return joinedAt > 0 ? DateTimeOffset.FromUnixTimeSeconds(joinedAt) : DateTimeOffset.UtcNow;
    }
}
