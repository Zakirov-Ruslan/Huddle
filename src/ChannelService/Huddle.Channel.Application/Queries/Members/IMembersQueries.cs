using Huddle.Channel.Application.Dto;
using Huddle.Channel.Domain;

namespace Huddle.Channel.Application.Queries.Members
{
    public interface IMembersQueries
    {
        Task<PaginatedItems<MemberDto>> GetByServerId(Guid serverId, Guid? cursor = null, int limit = 50);
        Task<MemberDto> GetAsync(Guid memberId);
    }
}
