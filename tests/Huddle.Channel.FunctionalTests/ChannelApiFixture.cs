using Huddle.Channel.Application.Services;
using Huddle.Channel.WebApi;
using Huddle.EventBus.Abstractions;
using Huddle.EventBusRabbitMQ;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;

namespace Huddle.Channel.FunctionalTests;

/// <summary>
/// Boots the real channel API on top of a PostgreSQL container. The database lives in Docker, everything that
/// would require another service (RabbitMQ, files, identity) is replaced by a test double.
/// </summary>
public sealed class ChannelApiFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string EventBusConnectionString = "amqp://guest:guest@localhost:5672";
    private const string PostgresImage = "postgres:latest";

    private readonly PostgreSqlContainer _database = new PostgreSqlBuilder()
        .WithImage(PostgresImage)
        .WithDatabase("channeldb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private string _connectionString = string.Empty;

    public ChannelApiFixture()
    {
        // Required by the API at startup, the value itself is never used: authentication is replaced below.
        Environment.SetEnvironmentVariable("IDENTITY_URL", "https://identity.test");
    }

    public async Task InitializeAsync()
    {
        await _database.StartAsync();

        _connectionString = _database.GetConnectionString();

        // Forces the host to start, which applies the EF Core migrations.
        _ = CreateClient();
    }

    public new async Task DisposeAsync()
    {
        Dispose();

        await _database.DisposeAsync();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(configuration => configuration.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["ConnectionStrings:channeldb"] = _connectionString,
                ["ConnectionStrings:eventbus"] = EventBusConnectionString,
            }));

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // The RabbitMQ event bus would try to reach a broker on startup and on every publish.
            RemoveRabbitMqEventBus(services);
            services.RemoveAll<IEventBus>();
            services.AddSingleton<IEventBus, NoopEventBus>();

            services.RemoveAll<IFilesClient>();
            services.AddSingleton<IFilesClient, FakeFilesClient>();

            services.AddTestAuthentication();
        });
    }

    private static void RemoveRabbitMqEventBus(IServiceCollection services)
    {
        var descriptors = services
            .Where(descriptor => descriptor.ServiceType == typeof(IHostedService) &&
                (descriptor.ImplementationType == typeof(RabbitMQEventBus) ||
                 descriptor.ImplementationFactory?.Method.DeclaringType?.FullName?
                     .StartsWith("Microsoft.Extensions.Hosting.RabbitMqDependencyInjectionExtensions", StringComparison.Ordinal) == true))
            .ToList();

        foreach (var descriptor in descriptors)
            services.Remove(descriptor);
    }
}

[CollectionDefinition(Name)]
public sealed class ChannelApiCollection : ICollectionFixture<ChannelApiFixture>
{
    public const string Name = "channel-api";
}
