using Huddle.Channel.Application.Dto;

namespace Huddle.Channel.Application.Queries.Members
{
    public interface IMembersQueries
    {
        Task<IEnumerable<MemberDto>> GetByServerId(Guid serverId);
        Task<MemberDto> GetAsync(Guid memberId);
    }
}
