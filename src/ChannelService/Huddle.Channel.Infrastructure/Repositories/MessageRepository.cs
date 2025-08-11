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

        public async Task<PaginatedItems<Message>> GetMessagesAsync(Guid channelId, Guid? cursor = null, int limit = 50)
        {
            var query = _context.Messages
                .Where(m => m.ChannelId == channelId)
                .OrderByDescending(m => m.SentAt);

            if (cursor.HasValue)
            {
                var cursorMessage = await _context.Messages.FindAsync(cursor.Value);
                if (cursorMessage is not null)
                {
                    query = _context.Messages
                        .Where(m => m.ChannelId == channelId && m.SentAt < cursorMessage.SentAt)
                        .OrderByDescending(m => m.SentAt);
                }
            }

            var messages = await query.Take(limit + 1).ToListAsync();

            var hasMore = messages.Count > limit;
            var result = hasMore ? messages.Take(limit).ToList() : messages;
            var nextCursor = result.Any() ? result.Last().Id : (Guid?)null;

            return new PaginatedItems<Message>(result, hasMore, nextCursor);
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
            var message = await _context.Messages.SingleOrDefaultAsync(m => m.Id == id)  ??
                throw new KeyNotFoundException($"Server with id {id} not found");

            _context.Messages.Remove(message);
        }
    }
}
