using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using Huddle.Channel.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Huddle.Channel.Infrastructure.Repositories
{
    public class ServerRepository : IServerRepository
    {
        private readonly ChannelContext _context;

        public ServerRepository(ChannelContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Server?> GetAsync(Guid severId)
        {
            return await _context.Servers
                .Include(s => s.Channels)
                .FirstOrDefaultAsync(s => s.Id == severId);
        }

        public async Task<IEnumerable<Server>> GetByMemberIdAsync(Guid memberId)
        {
            var serverIds = _context.Members
                .AsNoTracking()
                .Where(m => m.IdentityId == memberId)
                .Select(m => m.ServerId);

            return await _context.Servers.Where(s => serverIds.Contains(s.Id)).ToListAsync();
        }

        public Server Add(Server server)
        {
            _context.Servers.Add(server);

            return server;
        }

        public async Task Delete(Guid id)
        {
            var server = await _context.Servers.FindAsync(id)
                ?? throw new KeyNotFoundException($"Server with id {id} not found");

            _context.Servers.Remove(server);
        }

        public async Task<Domain.Aggregates.ServerAggregate.Channel?> GetChannelAsync(Guid channelId)
        {
            return await _context.Channels.FirstOrDefaultAsync(ch => ch.Id == channelId);
        }
    }
}
