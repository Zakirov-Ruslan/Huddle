using Huddle.Channel.Domain.SeedWork;

namespace Huddle.Channel.Domain.Aggregates.MemberAggregate
{
    public interface IMemberRepository : IRepository<Member>
    {
        Task<PaginatedItems<Member>> GetByServerIdAsync(Guid serverId, Guid? cursor = null, int limit = 50);
        Task<Member?> GetAsync(Guid memberId);
        Member Add(Member member);
        Task Delete(Guid memberId);
        Task<Member?> GetByServerAndIdentityIdAsync(Guid serverId, Guid identityId);
    }
}
