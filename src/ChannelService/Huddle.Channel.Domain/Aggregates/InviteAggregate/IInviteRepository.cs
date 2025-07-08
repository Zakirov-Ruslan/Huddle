using Huddle.Channel.Domain.SeedWork;

namespace Huddle.Channel.Domain.Aggregates.InviteAggregate
{
    public interface IInviteRepository : IRepository<Invite>
    {
        Task<IEnumerable<Invite>> GetByUserId(Guid identityId);
        Task<IEnumerable<Invite>> GetBySeverId(Guid serverId);
        Invite Add(Invite invite);
        Task Delete(Guid inviteId);
    }
}
