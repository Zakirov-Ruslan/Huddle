using Huddle.Channel.Application.Dto;

namespace Huddle.Channel.Application.Queries.Invites
{
    public interface IInvitesQueries
    {
        Task<IEnumerable<InviteDto>> GetInvitesByUserId(Guid identityId);
        Task<IEnumerable<InviteDto>> GetInvitesByServerId(Guid serverId);
    }
}
