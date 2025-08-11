using Huddle.Channel.Domain.SeedWork;

namespace Huddle.Channel.Domain.Aggregates.MessageAggregate
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<PaginatedItems<Message>> GetMessagesAsync(Guid channelId, Guid? cursor = null, int limit = 50);
        Task<Message?> GetAsync(Guid id);
        Message Add(Message message);
        Task Delete(Guid id);
    }
}
