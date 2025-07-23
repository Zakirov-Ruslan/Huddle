using Aspire.Hosting;

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

        var identity = builder.AddProject<Projects.Huddle_Identity>("identity")
            .WithReference(identityDb).WaitFor(identityDb);

        var identityEndpoint = identity.GetEndpoint("https");

        var channelService = builder.AddProject<Projects.Huddle_Channel_WebApi>("channel")
            .WithReference(channelDb).WaitFor(channelDb)
            .WithReference(rabbitMq).WaitFor(rabbitMq);

        builder.AddProject<Projects.Huddle_SignalR>("signalR")
            .WithReference(rabbitMq).WaitFor(rabbitMq)
            .WithReference(redis).WaitFor(redis);

        builder.AddProject<Projects.Huddle_Voice_WebApi>("voice")
            .WithReference(redis).WaitFor(redis);

        builder.AddProject<Projects.Huddle_ApiGateway>("gateway");

        var reactApp = builder.AddNpmApp("react-vite", "../Clients/huddle.react", "dev")
            .WithHttpEndpoint(env: "PORT")
            .WithExternalHttpEndpoints()
            .WithEnvironment("VITE_IDENTITY_URL", identityEndpoint);

        // Identity has a reference to all of the apps for callback urls, this is a cyclic reference
        identity.WithEnvironment("SPA_URL", reactApp.GetEndpoint("http"));
        channelService.WithEnvironment("SPA_URL", reactApp.GetEndpoint("http"))
            .WithEnvironment("IDENTITY_URL", identity.GetEndpoint("https"));

        builder.Build().Run();
    }
}