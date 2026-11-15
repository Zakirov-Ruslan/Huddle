using Huddle.FileService.Options;
using Huddle.FileService.Storage;
using Microsoft.Extensions.Options;

namespace Huddle.FileService.Services;

/// <summary>Creates the configured buckets on startup so uploads never fail on a fresh MinIO instance.</summary>
public class MinioBucketInitializer : IHostedService
{
    private readonly IFileStorage _storage;
    private readonly StorageOptions _options;
    private readonly ILogger<MinioBucketInitializer> _logger;

    public MinioBucketInitializer(IFileStorage storage, IOptions<StorageOptions> options, ILogger<MinioBucketInitializer> logger)
    {
        _storage = storage;
        _options = options.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var bucket in new[] { _options.AttachmentsBucket, _options.AvatarsBucket }.Distinct())
        {
            await _storage.EnsureBucketAsync(bucket, cancellationToken);
            _logger.LogInformation("MinIO bucket {Bucket} is ready", bucket);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
