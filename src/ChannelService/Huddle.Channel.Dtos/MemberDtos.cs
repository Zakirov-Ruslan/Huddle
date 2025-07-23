namespace Huddle.Channel.Application.Dto
{
    public record MemberDto
    (
        Guid Id,
        Guid ServerId,
        Guid IdentityId,
        string ServerUsername,
        string Description
    );

    public record UpdateMemberRequest
    (
        string ServerUsername,
        string Description
    );        
}
