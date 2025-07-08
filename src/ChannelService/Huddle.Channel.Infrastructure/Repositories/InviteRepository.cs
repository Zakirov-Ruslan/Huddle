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

        public async Task<IEnumerable<Invite>> GetByUserId(Guid identityId)
        {
            return await _context.Invites
                .Where(invite => invite.UserId == identityId)
                .ToListAsync();
        }

        public Invite Add(Invite invite)
        {
            return _context.Invites.Add(invite).Entity;
        }

        public async Task Delete(Guid inviteId)
        {
            var invite = await _context.Invites.FirstAsync(i => i.Id == inviteId);
            _context.Invites.Remove(invite);
        }
    }
}
