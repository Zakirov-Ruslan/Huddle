using Aspire.Hosting;
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