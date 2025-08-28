using Aspire.Hosting;
using Projects;

internal class Program
{
    private const string LIVEKIT_API_KEY = "api";
    private const string LIVEKIT_API_SECRET = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

    private static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        var redis = builder.AddRedis("redis");
        var rabbitMq = builder.AddRabbitMQ("eventbus")
            .WithLifetime(ContainerLifetime.Persistent);
        var postgres = builder.AddPostgres("postgres")
            .WithImageTag("latest")
            .WithLifetime(ContainerLifetime.Persistent);

        var channelDb = postgres.AddDatabase("channeldb");
        var identityDb = postgres.AddDatabase("identitydb");

        var minio = builder.AddMinioContainer("minio");
        var fileServise = builder.AddProject<Huddle_FileService>("minio-client")
            .WithReference(minio).WaitFor(minio);

        var identity = builder.AddProject<Projects.Huddle_Identity>("identity")
            .WithReference(identityDb).WaitFor(identityDb);

        var liveKit = builder.AddContainer("livekit", "livekit/livekit-server")
            .WithHttpEndpoint(port: 7880, targetPort: 7880, name: "http")
            .WithHttpEndpoint(port: 7881, targetPort: 7881, name: "websocket", isProxied: false)
            .WithEnvironment("REDIS_HOST", () => $"redis:6379")
            .WithEnvironment("REDIS_PASSWORD", () => redis.Resource.PasswordParameter.Value) 
            .WithEnvironment("LIVEKIT_KEYS", $"{LIVEKIT_API_KEY}: {LIVEKIT_API_SECRET}") 
                .WaitFor(redis);

        var identityEndpoint = identity.GetEndpoint("https");

        var channelService = builder.AddProject<Projects.Huddle_Channel_WebApi>("channel")
            .WithReference(channelDb).WaitFor(channelDb)
            .WithReference(rabbitMq).WaitFor(rabbitMq);

        var signalRService = builder.AddProject<Projects.Huddle_SignalR>("signalR")
            .WithReference(rabbitMq).WaitFor(rabbitMq)
            .WithReference(redis).WaitFor(redis)
            .WithEnvironment("IDENTITY_URL", identity.GetEndpoint("https"));

        var voiceService = builder.AddProject<Projects.Huddle_Voice_WebApi>("voice")
            .WithReference(redis).WaitFor(redis)
            .WithReference(rabbitMq).WaitFor(rabbitMq)
            .WithEnvironment("LIVEKIT_API_KEY", LIVEKIT_API_KEY)
            .WithEnvironment("LIVEKIT_API_SECRET", LIVEKIT_API_SECRET)
            .WithEnvironment("LIVEKIT_URL", liveKit.GetEndpoint("http"));

        var gateway = builder.AddProject<Projects.Huddle_ApiGateway>("gateway");

        var reactApp = builder.AddNpmApp("react-vite", "../Clients/huddle.react", "dev")
            .WithHttpEndpoint(env: "PORT")
            .WithExternalHttpEndpoints()
            .WithEnvironment("VITE_IDENTITY_URL", identityEndpoint)
            .WithEnvironment("VITE_GATEWAY_URL", gateway.GetEndpoint("https"));

        gateway.WithEnvironment("SPA_URL", reactApp.GetEndpoint("http"));
        identity.WithEnvironment("SPA_URL", reactApp.GetEndpoint("http"));
        channelService.WithEnvironment("SPA_URL", reactApp.GetEndpoint("http"))
            .WithEnvironment("IDENTITY_URL", identity.GetEndpoint("https"));
        signalRService.WithEnvironment("SPA_URL", reactApp.GetEndpoint("http"))
            .WithEnvironment("CHANNELS_URL", channelService.GetEndpoint("https"));
        voiceService.WithEnvironment("IDENTITY_URL", identity.GetEndpoint("https"))
            .WithEnvironment("CHANNELS_URL", channelService.GetEndpoint("https"));

        builder.Build().Run();
    }
}