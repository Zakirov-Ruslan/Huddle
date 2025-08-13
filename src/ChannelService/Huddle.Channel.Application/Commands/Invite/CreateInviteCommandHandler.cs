using AutoMapper;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Exceptions;
using Huddle.Channel.Application.Services;
using Huddle.Channel.Domain.Aggregates.InviteAggregate;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using MediatR;

namespace Huddle.Channel.Application.Commands.Invite
{
    public class CreateInviteCommandHandler : IRequestHandler<CreateInviteCommand, InviteDto>
    {
        private readonly IInviteRepository _inviteRepository;
        private readonly IServerRepository _serverRepository;
        private readonly IShortIdService _shortIdService;
        private readonly IMapper _mapper;

        public CreateInviteCommandHandler(IInviteRepository inviteRepository, IShortIdService shortIdService, IServerRepository serverRepository, IMapper mapper)
        {
            _inviteRepository = inviteRepository;
            _shortIdService = shortIdService;
            _serverRepository = serverRepository;
            _mapper = mapper;
        }

        public async Task<InviteDto> Handle(CreateInviteCommand request, CancellationToken cancellationToken)
        {
            var server = await _serverRepository.GetAsync(request.ServerId)
                ?? throw new KeyNotFoundException("Server not found");
            if (server.OwnerIdentityId != request.SenderId)
                throw new ForbiddenAccessException("User doesnt have rights to create invites on this server");

            var existingInvite = await _inviteRepository.GetBySeverId(request.ServerId);
            if (existingInvite is not null)
                return _mapper.Map<InviteDto>(existingInvite);

            bool isCodeUnique = false;
            string code = string.Empty;

            const int maxAttempts = 5;
            for (int i = 0; i < maxAttempts; i++)
            {
                code = _shortIdService.GetShortId();
                isCodeUnique = await _inviteRepository.GetByCode(code) is null;
                if (isCodeUnique)
                    break;
            }

            if (!isCodeUnique)
                throw new Exception("Too many collisions on creating invitational short code");

            var invite = new Domain.Aggregates.InviteAggregate.Invite(request.ServerId, code);

            _inviteRepository.Add(invite);

            await _inviteRepository.UnitOfWork.SaveEntitiesAsync();

            return _mapper.Map<InviteDto>(invite);
        }
    }
}
