using Huddle.Channel.Domain.SeedWork;

namespace Huddle.Channel.Domain.Aggregates.InviteAggregate
{
    public interface IInviteRepository : IRepository<Invite>
    {
        Task<IEnumerable<Invite>> GetBySeverId(Guid serverId);
        Task<Invite?> GetByCode(string code);
        Task<Invite?> GetAsync(Guid inviteId);
        Invite Add(Invite invite);
        Task Delete(Guid inviteId);
    }
}
