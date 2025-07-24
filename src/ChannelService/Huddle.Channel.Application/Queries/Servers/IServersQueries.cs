using Huddle.Channel.Application.Dto;

namespace Huddle.Channel.Application.Queries.Servers
{
    public interface IServersQueries
    {
        Task<ServerDto> GetServerAsync(Guid serverId, Guid identityId);
        Task<IEnumerable<ServerDto>> GetServersByMemberAsync(Guid memberId);
    }
}
