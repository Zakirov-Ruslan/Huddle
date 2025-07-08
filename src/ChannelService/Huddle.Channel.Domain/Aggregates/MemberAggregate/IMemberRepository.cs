using Huddle.Channel.Domain.SeedWork;

namespace Huddle.Channel.Domain.Aggregates.MemberAggregate
{
    public interface IMemberRepository : IRepository<Member>
    {
        Task<IEnumerable<Member>> GetByServerIdAsync(Guid serverId); //need pagination here
        Task<Member?> GetAsync(Guid memberId);
        Member Add(Member mebmer);
        void Update(Member mebmer);
        Task Delete(Guid memberId);
    }
}
