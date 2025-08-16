using Huddle.SignalR.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using StackExchange.Redis;

namespace Huddle.SignalR;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddRabbitMqEventBus("eventbus")
            .AddEventBusSubscriptions();

        var identityUrl = Environment.GetEnvironmentVariable("IDENTITY_URL")
            ?? throw new ArgumentNullException("IDENTITY_URL environment variable not defined");

        // https://learn.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-9.0
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

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });


        var redisConnectionString = builder.Configuration.GetConnectionString("redis")
            ?? throw new Exception("Redis connection string not defined");
        builder.Services.AddSignalR(hubOptions =>
        {
            hubOptions.EnableDetailedErrors = true;
        })
        .AddStackExchangeRedis(redisConnectionString, options =>
        {
            // here templates to redis-backplane
            // https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/signalr/redis-backplane.md?plain=1
            options.Configuration.ChannelPrefix = RedisChannel.Literal("SignalRService");
            options.Configuration.AbortOnConnectFail = false;
        });

        builder.Services.AddCors();

        var app = builder.Build();

        app.UseCors(options =>
        {
            var spaUrl = Environment.GetEnvironmentVariable("SPA_URL")
                ?? throw new ArgumentNullException("SPA_URL environment variable not defined");

            options.WithOrigins(spaUrl)
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHub<NotificationsHub>("/hub")
            .RequireAuthorization();

        app.Run();
    }
}

