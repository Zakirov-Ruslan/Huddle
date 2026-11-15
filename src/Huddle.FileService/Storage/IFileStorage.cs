namespace Huddle.FileService.Storage;

public interface IFileStorage
{
    Task EnsureBucketAsync(string bucket, CancellationToken cancellationToken);

    Task UploadAsync(string bucket, string objectKey, Stream content, long size, string contentType, CancellationToken cancellationToken);

    Task WriteToAsync(string bucket, string objectKey, Stream destination, CancellationToken cancellationToken);

    Task DeleteAsync(string bucket, string objectKey, CancellationToken cancellationToken);

    Task<string> CreatePresignedDownloadUrlAsync(string bucket, string objectKey, TimeSpan lifetime, string fileName);
}
