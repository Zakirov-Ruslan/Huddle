using Huddle.Channel.Domain.SeedWork;

namespace Huddle.Channel.Domain.Aggregates.ServerAggregate
{
    public interface IServerRepository : IRepository<Server>
    {
        Task<IEnumerable<Server>> GetByMemberIdAsync(Guid memberId);
        Task<Server?> GetAsync(Guid severId);
        Server Add(Server server);
        Task Delete(Guid id);

        Task<Channel?> GetChannelAsync(Guid channelId);
    }
}
