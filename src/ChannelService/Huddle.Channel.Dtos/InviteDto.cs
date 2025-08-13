namespace Huddle.Channel.Application.Dto
{
    public record InviteDto
    (
        Guid ServerId,
        Guid UserId
    );
}
