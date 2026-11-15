using Aspire.Hosting;
using Huddle.AppHost;
using Huddle.AppHost.LiveKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Projects;
using System.Net.Sockets;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        // Keys shared between the containers and the services are generated on every run,
        // so no credentials are kept in the repository (see DevSecret for the overrides).
        var liveKitApiKey = DevSecret.Resolve(builder.Configuration, "LiveKit:ApiKey", DevSecret.NewLiveKitApiKey());
        var liveKitApiSecret = DevSecret.Resolve(builder.Configuration, "LiveKit:ApiSecret", DevSecret.New());
        var filesInternalApiKey = DevSecret.Resolve(builder.Configuration, "Files:InternalApiKey", DevSecret.New());

        var redis = builder.AddRedis("redis");
        var rabbitMq = builder.AddRabbitMQ("eventbus")
            .WithLifetime(ContainerLifetime.Persistent);
        var postgres = builder.AddPostgres("postgres")
            .WithImageTag("latest")
            .WithLifetime(ContainerLifetime.Persistent);

        var channelDb = postgres.AddDatabase("channeldb");
        var identityDb = postgres.AddDatabase("identitydb");
        var fileDb = postgres.AddDatabase("filedb");

        var identity = builder.AddProject<Projects.Huddle_Identity>("identity")
            .WithReference(identityDb).WaitFor(identityDb);

        var minio = builder.AddMinioContainer("minio")
            .WithDataVolume()
            .WithLifetime(ContainerLifetime.Persistent)
            .WithExternalHttpEndpoints();

        var files = builder.AddProject<Huddle_FileService>("files")
            .WithReference(minio).WaitFor(minio)
            .WithReference(fileDb).WaitFor(fileDb)
            .WithEnvironment("Storage__InternalApiKey", filesInternalApiKey)
            .WithEnvironment("IDENTITY_URL", identity.GetEndpoint("https"));

        //https://livekit.io/connection-test - for tests
        var liveKitOptions = builder.Configuration.GetSection(LiveKitOptions.SectionName).Get<LiveKitOptions>()
            ?? new LiveKitOptions();

        var liveKit = builder.AddContainer("livekit", "livekit/livekit-server")
            .WithHttpEndpoint(port: 7880, targetPort: 7880, name: "http")
            .WithEndpoint(port: 7881, targetPort: 7881, name: "ice-tcp", protocol: ProtocolType.Tcp)
            .WithEnvironment("LIVEKIT_CONFIG", LiveKitConfigBuilder.Build(liveKitOptions, builder.Environment.IsDevelopment()))
            .WithEnvironment("LIVEKIT_KEYS", $"{liveKitApiKey}: {liveKitApiSecret}")
            // Host networking keeps the RTC ports reachable without publishing the whole UDP range.
            // Docker Desktop provides the "host" network inside its VM, so there the ports still have to
            // be published or forwarded explicitly.
            // Redis is not wired up: a single LiveKit node serves the dev environment, multi-node setups can be
            // added to the generated config (see LiveKitConfigBuilder) together with a redis section.
            .WithContainerRuntimeArgs("--network", "host")
            .WithArgs("--dev");

        var identityEndpoint = identity.GetEndpoint("https");

        var channelService = builder.AddProject<Projects.Huddle_Channel_WebApi>("channel")
            .WithReference(channelDb).WaitFor(channelDb)
            .WithReference(rabbitMq).WaitFor(rabbitMq)
            .WithReference(files).WaitFor(files)
            .WithEnvironment("FILES_INTERNAL_API_KEY", filesInternalApiKey);

        var signalRService = builder.AddProject<Projects.Huddle_SignalR>("signalR")
            .WithReference(rabbitMq).WaitFor(rabbitMq)
            .WithReference(redis).WaitFor(redis)
            .WithEnvironment("IDENTITY_URL", identity.GetEndpoint("https"));

        var voiceService = builder.AddProject<Projects.Huddle_Voice_WebApi>("voice")
            .WithReference(redis).WaitFor(redis)
            .WithReference(rabbitMq).WaitFor(rabbitMq)
            .WithEnvironment("LIVEKIT_API_KEY", liveKitApiKey)
            .WithEnvironment("LIVEKIT_API_SECRET", liveKitApiSecret)
            .WithEnvironment("LIVEKIT_URL", liveKit.GetEndpoint("http"));

        var gateway = builder.AddProject<Projects.Huddle_ApiGateway>("gateway")
            .WithReference(files).WaitFor(files);

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
