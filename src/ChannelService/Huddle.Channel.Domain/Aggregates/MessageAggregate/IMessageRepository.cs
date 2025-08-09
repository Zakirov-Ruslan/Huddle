using Huddle.Channel.Domain.SeedWork;

namespace Huddle.Channel.Domain.Aggregates.MessageAggregate
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<PaginatedItems<Message>> GetMessagesAsync(Guid? cursor = null, int limit = 50);
        Task<Message?> GetAsync(Guid id);
        Message Add(Message message);
        Task Delete(Guid id);
    }

    public class PaginatedItems<T>
    {
        public IEnumerable<T> Items { get; set; }
        public bool HasMore { get; set; }
        public Guid? NextCursor { get; set; }

        public PaginatedItems(IEnumerable<T> items, bool hasMore, Guid? nextCursor)
        {
            Items = items;
            HasMore = hasMore;
            NextCursor = nextCursor;
        }
    }
}
