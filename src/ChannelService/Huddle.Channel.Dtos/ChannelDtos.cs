namespace Huddle.Channel.Application.Dto
{
    public record ChannelDto
    (
        Guid Id,
        Guid ServerId,
        string Name,
        string ChannelType
    );

    public record CreateChannelRequest
    (
        string Name,
        string ChannelType
    );

    public record UpdatedChannelRequest
    (
        Guid Id,
        string Name,
        string ChannelType
    );
}
