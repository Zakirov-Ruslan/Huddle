using AutoMapper;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;

namespace Huddle.Channel.Application.Queries.Servers
{
    public class ServersQueries : IServersQueries
    {
        private readonly IServerRepository _serverRepository;
        private readonly IMapper _mapper;

        public ServersQueries(IServerRepository serverRepository, IMapper mapper)
        {
            _serverRepository = serverRepository;
            _mapper = mapper;
        }

        public async Task<ServerDto> GetServerAsync(Guid id)
        {
            var server = await _serverRepository.GetAsync(id)
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
