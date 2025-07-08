using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChannelService.FunctionalTests
{
    public sealed class ServerApiFixture : IClassFixture<ServerApiFixture>
    {
        //private readonly IHost _app;

        //public IResourceBuilder<PostgresServerResource> Postgres { get; private set; }
        //public IResourceBuilder<PostgresServerResource> IdentityDB { get; private set; }
        //public IResourceBuilder<ProjectResource> IdentityApi { get; private set; }

        //private string _postgresConnectionString;

        //public ServerApiFixture()
        //{
        //    var options = new DistributedApplicationOptions { AssemblyName = typeof(ServerApiFixture).Assembly.FullName, DisableDashboard = true };
        //    var appBuilder = DistributedApplication.CreateBuilder(options);
        //    Postgres = appBuilder.AddPostgres("OrderingDB");
        //    IdentityDB = appBuilder.AddPostgres("IdentityDB");
        //    IdentityApi = appBuilder.AddProject<Projects.Huddle_Identity_WebApi>("identity-api").WithReference(IdentityDB);
        //    _app = appBuilder.Build();
        //}

        //protected override IHost CreateHost(IHostBuilder builder)
        //{
        //    builder.ConfigureHostConfiguration(config =>
        //    {
        //        config.AddInMemoryCollection(new Dictionary<string, string>
        //    {
        //        { $"ConnectionStrings:{Postgres.Resource.Name}", _postgresConnectionString },
        //        { "Identity:Url", IdentityApi.GetEndpoint("http").Url }
        //    });
        //    });
        //    builder.ConfigureServices(services =>
        //    {
        //        services.AddSingleton<IStartupFilter>(new AutoAuthorizeStartupFilter());
        //    });
        //    return base.CreateHost(builder);
        //}

        //public new async Task DisposeAsync()
        //{
        //    await base.DisposeAsync();
        //    await _app.StopAsync();
        //    if (_app is IAsyncDisposable asyncDisposable)
        //    {
        //        await asyncDisposable.DisposeAsync().ConfigureAwait(false);
        //    }
        //    else
        //    {
        //        _app.Dispose();
        //    }
        //}

        //public async Task InitializeAsync()
        //{
        //    await _app.StartAsync();
        //    _postgresConnectionString = await Postgres.Resource.GetConnectionStringAsync();
        //}

        //private class AutoAuthorizeStartupFilter : IStartupFilter
        //{
        //    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        //    {
        //        return builder =>
        //        {
        //            builder.UseMiddleware<AutoAuthorizeMiddleware>();
        //            next(builder);
        //        };
        //    }
        //}
    }
}
