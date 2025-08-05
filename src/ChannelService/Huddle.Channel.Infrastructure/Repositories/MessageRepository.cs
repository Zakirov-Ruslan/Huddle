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

        public async Task<IEnumerable<Message>> GetRecentAsync(Guid chatId, int pageSize)
        {
            var messages = await _context.Messages
                .AsNoTracking()
                .Where(m => m.ChannelId == chatId)
                .OrderByDescending(m => m.SentAt)
                .Take(pageSize)
                .ToListAsync();

            return messages;
        }

        public async Task<IEnumerable<Message>> GetOlderAsync(Guid chatId, int pageSize, Guid beforeThan)
        {
            var higherMessage = await _context.Messages.FirstAsync(m => m.Id == beforeThan);

            var messages = await _context.Messages
                .AsNoTracking()
                .Where(m => m.ChannelId == chatId && m.SentAt < higherMessage.SentAt)
                .Take(pageSize)
                .ToListAsync();

            return messages;
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

        public void Update(Message message)
        {
            _context.Update(message);
        }

        public async Task Delete(Guid id)
        {
            var message = await _context.Messages.SingleOrDefaultAsync(m => m.Id == id)  ??
                throw new KeyNotFoundException($"Server with id {id} not found");

            _context.Messages.Remove(message);
        }
    }
}
