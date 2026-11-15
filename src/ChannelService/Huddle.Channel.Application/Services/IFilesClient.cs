namespace Huddle.Channel.Application.Services
{
    public record ClaimedFile
    (
        Guid Id,
        string FileName,
        string ContentType,
        long Size,
        bool IsImage
    );

    /// <summary>Client for the file service, used to verify and reserve uploaded files.</summary>
    public interface IFilesClient
    {
        Task<IReadOnlyList<ClaimedFile>> ClaimAttachmentsAsync(Guid ownerId, IReadOnlyList<Guid> fileIds, CancellationToken cancellationToken = default);

        Task<ClaimedFile> ClaimAvatarAsync(Guid ownerId, Guid fileId, CancellationToken cancellationToken = default);
    }
}
