using Huddle.Channel.Domain.SeedWork;

namespace Huddle.Channel.Domain.Aggregates.MessageAggregate
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<IEnumerable<Message>> GetRecentAsync(Guid chatId, int pageSize);
        Task<IEnumerable<Message>> GetOlderAsync(Guid chatId, int pageSize, Guid beforeThan);
        Task<Message?> GetAsync(Guid id);
        Message Add(Message message);
        void Update(Message message);
        Task Delete(Guid id);
    }
}
