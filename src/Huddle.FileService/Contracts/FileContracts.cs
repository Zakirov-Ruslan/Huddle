using Huddle.FileService.Data;

namespace Huddle.FileService.Contracts;

public record FileDto
(
    Guid Id,
    string Url,
    string FileName,
    string ContentType,
    long Size,
    bool IsImage
);

public record PresignedUrlDto
(
    string Url,
    DateTime ExpiresAt
);

public record ClaimFilesRequest
(
    Guid OwnerId,
    FileKind Kind,
    IReadOnlyList<Guid> Ids
);

public record ClaimedFileDto
(
    Guid Id,
    string FileName,
    string ContentType,
    long Size,
    bool IsImage
);

public record ClaimFilesResponse
(
    IReadOnlyList<ClaimedFileDto> Files,
    IReadOnlyList<Guid> MissingIds
);
