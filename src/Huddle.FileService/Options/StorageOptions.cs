namespace Huddle.FileService.Options;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";
    public const string MinioConnectionStringName = "minio";

    public string? Endpoint { get; set; }
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public bool UseSsl { get; set; }

    public string AttachmentsBucket { get; set; } = "attachments";
    public string AvatarsBucket { get; set; } = "avatars";

    /// <summary>Path clients use to reach the files API through the gateway.</summary>
    public string PublicBasePath { get; set; } = "/files/api/files";

    public int PresignedUrlLifetimeMinutes { get; set; } = 15;

    public long MaxAttachmentSizeBytes { get; set; } = 25L * 1024 * 1024;
    public long MaxAvatarSizeBytes { get; set; } = 5L * 1024 * 1024;
    public int MaxAttachmentsPerMessage { get; set; } = 10;

    public int PendingFileLifetimeHours { get; set; } = 24;

    /// <summary>Shared secret used by internal services (ChannelService) to claim files.</summary>
    public string InternalApiKey { get; set; } = string.Empty;

    public string GetBucket(Data.FileKind kind) => kind == Data.FileKind.Avatar ? AvatarsBucket : AttachmentsBucket;

    public long GetMaxSize(Data.FileKind kind) => kind == Data.FileKind.Avatar ? MaxAvatarSizeBytes : MaxAttachmentSizeBytes;

    public string BuildPublicUrl(Guid fileId) => $"{PublicBasePath.TrimEnd('/')}/{fileId}/content";
}
