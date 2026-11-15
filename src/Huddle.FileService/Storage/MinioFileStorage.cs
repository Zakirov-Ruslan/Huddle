using Minio;
using Minio.DataModel.Args;

namespace Huddle.FileService.Storage;

public sealed class MinioFileStorage : IFileStorage
{
    private readonly IMinioClient _client;

    public MinioFileStorage(IMinioClient client)
    {
        _client = client;
    }

    public async Task EnsureBucketAsync(string bucket, CancellationToken cancellationToken)
    {
        var exists = await _client.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(bucket), cancellationToken);

        if (exists)
            return;

        await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket), cancellationToken);
    }

    public async Task UploadAsync(string bucket, string objectKey, Stream content, long size, string contentType, CancellationToken cancellationToken)
    {
        var args = new PutObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectKey)
            .WithStreamData(content)
            .WithObjectSize(size)
            .WithContentType(contentType);

        await _client.PutObjectAsync(args, cancellationToken);
    }

    public async Task WriteToAsync(string bucket, string objectKey, Stream destination, CancellationToken cancellationToken)
    {
        var args = new GetObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectKey)
            .WithCallbackStream(content => content.CopyTo(destination));

        await _client.GetObjectAsync(args, cancellationToken);
    }

    public Task DeleteAsync(string bucket, string objectKey, CancellationToken cancellationToken)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectKey);

        return _client.RemoveObjectAsync(args, cancellationToken);
    }

    public Task<string> CreatePresignedDownloadUrlAsync(string bucket, string objectKey, TimeSpan lifetime, string fileName)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectKey)
            .WithExpiry((int)lifetime.TotalSeconds);

        return _client.PresignedGetObjectAsync(args);
    }
}
