namespace Huddle.FileService.Data;

public enum FileKind
{
    Attachment,
    Avatar
}

public enum FileStatus
{
    /// <summary>Uploaded, but not yet referenced by a message or an avatar.</summary>
    Pending,
    Attached
}

public class StoredFile
{
    public Guid Id { get; private set; }
    public Guid OwnerId { get; private set; }
    public FileKind Kind { get; private set; }
    public FileStatus Status { get; private set; }
    public string Bucket { get; private set; } = null!;
    public string ObjectKey { get; private set; } = null!;
    public string FileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long Size { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private StoredFile() { }

    public StoredFile(Guid id, Guid ownerId, FileKind kind, string bucket, string objectKey, string fileName, string contentType, long size)
    {
        Id = id;
        OwnerId = ownerId;
        Kind = kind;
        Status = FileStatus.Pending;
        Bucket = bucket;
        ObjectKey = objectKey;
        FileName = fileName;
        ContentType = contentType;
        Size = size;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAttached() => Status = FileStatus.Attached;
}
