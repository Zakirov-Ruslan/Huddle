using Huddle.Grpc;
using Huddle.Voice.WebApi.Services;
using Livekit.Server.Sdk.Dotnet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace Huddle.Voice.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var SERVER_PORT = builder.Configuration.GetValue<int>("SERVER_PORT");
        var LIVEKIT_URL = builder.Configuration.GetValue<string>("LIVEKIT_URL");
        var LIVEKIT_API_KEY = builder.Configuration.GetValue<string>("LIVEKIT_API_KEY");
        var LIVEKIT_API_SECRET = builder.Configuration.GetValue<string>("LIVEKIT_API_SECRET");

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

        // GRPC
        // https://learn.microsoft.com/ru-ru/aspnet/core/grpc/loadbalancing?view=aspnetcore-9.0 - Load balancing
        builder.Services.AddGrpcClient<ChannelAccess.ChannelAccessClient>(options =>
        {
            options.Address = new Uri(Environment.GetEnvironmentVariable("CHANNELS_URL")
                ?? throw new ArgumentNullException("CHANNELS_URL environment variable not defined"));
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        // Access Token generator
        app.MapGet(
            "/livekit/token",
            async ([FromQuery] string serverId,
                   [FromQuery] string channelId,
                   HttpContext context,
                   GrpcChannelAccessClient grpcChannelAccessClient,
                   ILogger<Program> logger) =>
            {
                var user = context.User;
                if (user.Identity?.IsAuthenticated != true)
                    return Results.Unauthorized();

                var userId = user.FindFirst("sub")?.Value;
                if (userId == null)
                    return Results.BadRequest("User ID not found");

                logger.LogInformation("Запрос на токен от пользователя {userId} для комнаты {channelId}", userId, channelId);

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

                var apiKey = builder.Configuration["LiveKit:ApiKey"];
                var apiSecret = builder.Configuration["LiveKit:ApiSecret"];
                var roomName = $"server-{serverId}-channel-{channelId}";
                var displayName = user.FindFirst("name")?.Value ?? userId;

                var token = new AccessToken(LIVEKIT_API_KEY, LIVEKIT_API_SECRET)
                    .WithIdentity(userId)
                    .WithName(displayName)
                    .WithGrants(new VideoGrants { RoomJoin = true, Room = roomName })
                    .WithAttributes(new Dictionary<string, string> { { "mykey", "myvalue" } })
                    .WithTtl(TimeSpan.FromHours(1));

                logger.LogInformation("Token successfuly generated for user {userId} and channel {channelId}", userId, channelId);

                return Results.Ok(token.ToJwt());
            }
        )
        .RequireAuthorization();

        // Webhook handler
        // https://docs.livekit.io/home/server/webhooks/
        var webhookReceiver = new WebhookReceiver(LIVEKIT_API_KEY, LIVEKIT_API_SECRET);
        app.MapPost(
            "/livekit/webhook",
            async (HttpRequest request) =>
            {
                var body = new StreamReader(request.Body);
                string postData = await body.ReadToEndAsync();

                var authHeader = request.Headers["Authorization"];
                if (authHeader.Count == 0)
                {
                    return Results.BadRequest("Authorization header is required");
                }
                WebhookEvent webhookEvent = webhookReceiver.Receive(postData, authHeader.First());

                switch (webhookEvent.Event)
                {
                    case "participant_joined": 
                        {
                            //var @event = new VoiceParticipantJoined(
                            //    ServerId: ExtractServerId(e.Room),
                            //    ChannelId: ExtractChannelId(e.Room),
                            //    UserId: e.Participant.Identity
                            //);
                            //await _eventBus.PublishAsync(@event);
                            return Results.Ok();
                        }
                    case "participant_left":
                        {
                            return Results.Ok();
                        }
                    case "room_started":
                    case "room_finished":
                    case "participant_connection_aborted":
                    case "track_published":
                    case "track_unpublished":
                    case "egress_started":
                    case "egress_updated":
                    case "egress_ended":
                    case "ingress_started":
                    case "ingress_ended":
                    default:
                        break;
                }

                return Results.Ok();
            }
        ).RequireAuthorization();

        app.Run();
    }
}
