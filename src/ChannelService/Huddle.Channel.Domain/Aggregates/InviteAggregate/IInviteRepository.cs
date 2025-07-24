using Huddle.Channel.Domain.SeedWork;

namespace Huddle.Channel.Domain.Aggregates.InviteAggregate
{
    public interface IInviteRepository : IRepository<Invite>
    {
        Task<IEnumerable<Invite>> GetByUserId(Guid identityId);
        Task<IEnumerable<Invite>> GetBySeverId(Guid serverId);
        Task<Invite?> GetAsync(Guid inviteId);
        Invite Add(Invite invite);
        Task Delete(Guid inviteId);
    }
}
