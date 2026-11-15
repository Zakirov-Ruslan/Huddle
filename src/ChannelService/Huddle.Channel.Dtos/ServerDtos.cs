namespace Huddle.Channel.Application.Dto
{
    public record ServerDto
    (
        Guid Id,
        string Name,
        Guid OwnerIdentityId,
        string? AvatarUrl,
        List<ChannelDto> Channels
    );

    public record CreateServerRequest
    (
        string Name,
        bool isPrivate,
        Guid? AvatarFileId
    );

    public record UpdateServerRequest
    (
        string Name,
        Guid? AvatarFileId,
        bool RemoveAvatar
    );

    public record UpdateServerAvatarRequest
    (
        Guid FileId
    );
}
