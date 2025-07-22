namespace Huddle.Channel.Application.Dto
{
    public record InviteDto
    (
        Guid ServerId,
        Guid UserId
    );

    public record CreateInviteRequest
    (
        Guid ServerId,
        Guid UserId
    );
}
