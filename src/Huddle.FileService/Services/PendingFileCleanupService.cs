using Huddle.FileService.Data;
using Huddle.FileService.Options;
using Huddle.FileService.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Huddle.FileService.Services;

/// <summary>Removes uploads that were never referenced by a message or an avatar.</summary>
public class PendingFileCleanupService : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IFileStorage _storage;
    private readonly StorageOptions _options;
    private readonly ILogger<PendingFileCleanupService> _logger;

    public PendingFileCleanupService(
        IServiceScopeFactory scopeFactory,
        IFileStorage storage,
        IOptions<StorageOptions> options,
        ILogger<PendingFileCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _storage = storage;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);

        do
        {
            try
            {
                await CleanupAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cleanup pending files");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task CleanupAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<FileDbContext>();

        var threshold = DateTime.UtcNow.AddHours(-_options.PendingFileLifetimeHours);

        var staleFiles = await db.Files
            .Where(f => f.Status == FileStatus.Pending && f.CreatedAt < threshold)
            .ToListAsync(cancellationToken);

        if (staleFiles.Count == 0)
            return;

        foreach (var file in staleFiles)
        {
            await _storage.DeleteAsync(file.Bucket, file.ObjectKey, cancellationToken);
            db.Files.Remove(file);
        }

        await db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Removed {Count} stale pending files", staleFiles.Count);
    }
}
