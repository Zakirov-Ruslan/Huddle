namespace Huddle.Channel.Application.Dto
{
    public record ServerDto
    (
        Guid Id,
        string Name,
        Guid OwnerIdentityId,
        List<ChannelDto> Channels
    );

    public record CreateServerRequest
    (
        string Name,
        bool isPrivate
    );

    public record UpdateServerRequest
    (
        string Name
    );
}
