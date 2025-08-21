using Projects;

internal class Program
{
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

        var liveKit = builder.AddContainer("LiveKit", "livekit/livekit-server")
            .WithHttpEndpoint(port: 7880, targetPort: 7880, name: "http")
            .WithHttpEndpoint(port: 7881, targetPort: 7881, name: "websocket", isProxied: false)
            .WithEnvironment("LIVEKIT_API_KEY", "your-livekit-api-key")
            .WithEnvironment("LIVEKIT_API_SECRET", "your-livekit-api-secret")
            //.WithEnvironment("REDIS_ADDRESS", () => redis.GetConnectionString()) // Подключение к Redis
            .WaitFor(redis) // Ждём Redis
            .WithEnvironment("PORT", "7880")
            .WithEnvironment("WS_PORT", "7881");

        var identityEndpoint = identity.GetEndpoint("https");

        var channelService = builder.AddProject<Projects.Huddle_Channel_WebApi>("channel")
            .WithReference(channelDb).WaitFor(channelDb)
            .WithReference(rabbitMq).WaitFor(rabbitMq);

        var signalRService = builder.AddProject<Projects.Huddle_SignalR>("signalR")
            .WithReference(rabbitMq).WaitFor(rabbitMq)
            .WithReference(redis).WaitFor(redis)
            .WithEnvironment("IDENTITY_URL", identity.GetEndpoint("https"));

        builder.AddProject<Projects.Huddle_Voice_WebApi>("voice")
            .WithReference(redis).WaitFor(redis);

        var gateway = builder.AddProject<Projects.Huddle_ApiGateway>("gateway");

        var reactApp = builder.AddNpmApp("react-vite", "../Clients/huddle.react", "dev")
            .WithHttpEndpoint(env: "PORT")
            .WithExternalHttpEndpoints()
            .WithEnvironment("VITE_IDENTITY_URL", identityEndpoint)
            .WithEnvironment("VITE_GATEWAY_URL", gateway.GetEndpoint("https"));

        identity.WithEnvironment("SPA_URL", reactApp.GetEndpoint("http"));
        channelService.WithEnvironment("SPA_URL", reactApp.GetEndpoint("http"))
            .WithEnvironment("IDENTITY_URL", identity.GetEndpoint("https"));
        signalRService.WithEnvironment("SPA_URL", reactApp.GetEndpoint("http"))
            .WithEnvironment("CHANNELS_URL", channelService.GetEndpoint("https"));

        builder.Build().Run();
    }
}