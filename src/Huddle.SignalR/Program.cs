using Huddle.SignalR.Extensions;
using StackExchange.Redis;

namespace Huddle.SignalR;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddRabbitMqEventBus("eventbus")
            .AddEventBusSubscriptions();

        //builder.Services.AddAuthentication();
        builder.Services.AddSignalR(hubOptions =>
        {
            hubOptions.EnableDetailedErrors = true;
        })
        .AddStackExchangeRedis("redis", options => 
        {
            // here templates to redis-backplane
            // https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/signalr/redis-backplane.md?plain=1
            options.Configuration.ChannelPrefix = RedisChannel.Literal("SignalRService");
        });
        //.AddJwtBearer();

        var app = builder.Build();

        app.Run();
    }
}

