namespace Huddle.Channel.Application.Dto
{
    public record InviteDto
    (
        Guid Id,
        Guid ServerId,
        DateTime CreatedAt,
        string Code
    );
}
