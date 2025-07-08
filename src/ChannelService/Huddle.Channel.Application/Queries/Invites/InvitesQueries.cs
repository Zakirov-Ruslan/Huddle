using AutoMapper;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Domain.Aggregates.InviteAggregate;

namespace Huddle.Channel.Application.Queries.Invites
{
    public class InvitesQueries : IInvitesQueries
    {
        private readonly IInviteRepository _invitesRepository;
        private readonly IMapper _mapper;

        public InvitesQueries(IInviteRepository invitesRepository, IMapper mapper)
        {
            _invitesRepository = invitesRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InviteDto>> GetInvitesByServerId(Guid serverId)
        {
            var invites = await _invitesRepository.GetBySeverId(serverId);
            return invites.Select(_mapper.Map<InviteDto>);
        }

        public async Task<IEnumerable<InviteDto>> GetInvitesByUserId(Guid identityId)
        {
            var invites = await _invitesRepository.GetByUserId(identityId);
            return invites.Select(_mapper.Map<InviteDto>);
        }
    }
}
