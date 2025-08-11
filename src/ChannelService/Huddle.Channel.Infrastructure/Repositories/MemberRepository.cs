using Huddle.Channel.Domain;
using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using Huddle.Channel.Domain.Aggregates.MessageAggregate;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
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

        public async Task<PaginatedItems<Member>> GetByServerIdAsync(Guid serverId, Guid? cursor = null, int limit = 50)
        {
            var query = _context.Members
                .Where(m => m.ServerId == serverId);

            if (cursor.HasValue)
            {
                var cursorMessage = await _context.Members.FindAsync(cursor.Value);
                if (cursorMessage is not null)
                {
                    query = _context.Members
                        .Where(m => m.ServerId == serverId && m.CreatedAt < cursorMessage.CreatedAt); 
                }
            }

            var messages = await query.Take(limit + 1).ToListAsync();

            var hasMore = messages.Count > limit;
            var result = hasMore ? messages.Take(limit).ToList() : messages;
            var nextCursor = result.Any() ? result.Last().Id : (Guid?)null;

            return new PaginatedItems<Member>(result, hasMore, nextCursor);
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
