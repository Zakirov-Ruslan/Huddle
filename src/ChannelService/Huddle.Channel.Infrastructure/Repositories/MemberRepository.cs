using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using Huddle.Channel.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Huddle.Channel.Infrastructure.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly ChannelContext _context;

        public MemberRepository(ChannelContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Member?> GetAsync(Guid memberId)
        {
            return await _context.Members.FirstOrDefaultAsync(invite => invite.Id == memberId);
        }

        public async Task<IEnumerable<Member>> GetByServerIdAsync(Guid serverId)
        {
            return await _context.Members
                .Where(invite => invite.ServerId == serverId)
                .ToListAsync();
        }

        public Member Add(Member mebmer)
        {
            return _context.Add(mebmer).Entity;
        }

        public async Task Delete(Guid memberId)
        {
            var invite = await _context.Members.FirstAsync(invite => invite.Id == memberId);
            _context.Members.Remove(invite);
        }

        public async Task<Member?> GetByServerAndIdentityIdAsync(Guid serverId, Guid identityId)
        {
            return await _context.Members.FirstOrDefaultAsync(m => m.ServerId == serverId && m.IdentityId == identityId);
        }
    }
}
