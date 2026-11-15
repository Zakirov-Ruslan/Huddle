using Huddle.FileService.Data;
using Microsoft.EntityFrameworkCore;

namespace Huddle.FileService.Services;

/// <summary>
/// Applies migrations synchronously during startup, so every other hosted service sees the schema.
/// </summary>
public class DatabaseMigrationHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DatabaseMigrationHostedService> _logger;

    public DatabaseMigrationHostedService(IServiceScopeFactory scopeFactory, ILogger<DatabaseMigrationHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<FileDbContext>();

        _logger.LogInformation("Migrating database associated with context {DbContextName}", nameof(FileDbContext));

        await context.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
