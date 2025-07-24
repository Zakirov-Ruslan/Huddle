using AutoMapper;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Exceptions;
using Huddle.Channel.Application.Services;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;

namespace Huddle.Channel.Application.Queries.Servers
{
    public class ServersQueries : IServersQueries
    {
        private readonly IServerRepository _serverRepository;
        private readonly IMapper _mapper;
        private readonly IAccessService _accessService;

        public ServersQueries(IServerRepository serverRepository, IMapper mapper, IAccessService accessService)
        {
            _serverRepository = serverRepository;
            _mapper = mapper;
            _accessService = accessService;
        }

        public async Task<ServerDto> GetServerAsync(Guid serverId, Guid identityId)
        {
            var hasAccess = await _accessService.IsUserMemberOfServerAsync(serverId, identityId);
            if (!hasAccess)
                throw new ForbiddenAccessException("User is not a member of this server");

            var server = await _serverRepository.GetAsync(serverId)
                ?? throw new KeyNotFoundException("Server not found");

            return _mapper.Map<ServerDto>(server);
        }

        public async Task<IEnumerable<ServerDto>> GetServersByMemberAsync(Guid memberId)
        {
            var servers = await _serverRepository.GetByMemberIdAsync(memberId);

            return servers.Select(_mapper.Map<ServerDto>);
        }
    }
}
