using Huddle.Channel.Domain;
using Huddle.Channel.Domain.Aggregates.MessageAggregate;
using Huddle.Channel.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Huddle.Channel.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ChannelContext _context;

        public MessageRepository(ChannelContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<PaginatedItems<Message>> GetMessagesAsync(Guid channelId, Guid? cursor = null, bool older = true, int limit = 50)
        {
            var query = _context.Messages
                .Where(m => m.ChannelId == channelId);

            if (cursor.HasValue)
            {
                query = older
                    ? query.Where(m => m.Id < cursor.Value)
                    : query.Where(m => m.Id > cursor.Value);
            }

            query = query.OrderByDescending(m => m.Id);

            var messages = await query.Take(limit + 1).ToListAsync();

            var hasMore = messages.Count > limit;
            bool hasNext = !older && hasMore;
            bool hasPrev = older && hasMore;

            var result = hasMore ? messages.Take(limit).ToList() : messages;

            var nextCursor = result.Any()
                ? older ? result.First().Id : result.Last().Id
                : (Guid?)null;

            var prevCursor = result.Any()
                ? older ? result.Last().Id : result.First().Id
                : (Guid?)null;

            return new PaginatedItems<Message>(result, hasPrev, hasNext, nextCursor, prevCursor);
        }

        public async Task<Message?> GetAsync(Guid id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public Message Add(Message message)
        {
            _context.Messages.Add(message);

            return message;
        }

        public async Task Delete(Guid id)
        {
            var message = await _context.Messages.SingleOrDefaultAsync(m => m.Id == id) ??
                throw new KeyNotFoundException($"Server with id {id} not found");

            _context.Messages.Remove(message);
        }
    }
}
