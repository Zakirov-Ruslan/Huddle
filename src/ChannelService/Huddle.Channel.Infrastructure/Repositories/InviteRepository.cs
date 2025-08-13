using Huddle.Channel.Domain.Aggregates.InviteAggregate;
using Huddle.Channel.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Huddle.Channel.Infrastructure.Repositories
{
    public class InviteRepository : IInviteRepository
    {
        private readonly ChannelContext _context;

        public InviteRepository(ChannelContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<IEnumerable<Invite>> GetBySeverId(Guid serverId)
        {
            return await _context.Invites
                .Where(invite => invite.ServerId == serverId)
                .ToListAsync();
        }

        public async Task<Invite?> GetAsync(Guid inviteId)
        {
            return await _context.Invites.FirstOrDefaultAsync(i => i.Id == inviteId);
        }

        public async Task<Invite?> GetByCode(string code)
        {
            return await _context.Invites
                .SingleOrDefaultAsync(invite => invite.Code == code);
        }

        public Invite Add(Invite invite)
        {
            return _context.Invites.Add(invite).Entity;
        }

        public async Task Delete(Guid inviteId)
        {
            var invite = await _context.Invites.FindAsync(inviteId)
                ?? throw new KeyNotFoundException("Invite not found");
            _context.Invites.Remove(invite);
        }
    }
}
