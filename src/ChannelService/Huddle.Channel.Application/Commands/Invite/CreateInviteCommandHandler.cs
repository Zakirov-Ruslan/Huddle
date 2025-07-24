using Huddle.Channel.Domain.Aggregates.InviteAggregate;
using MediatR;

namespace Huddle.Channel.Application.Commands.Invite
{
    public class CreateInviteCommandHandler : IRequestHandler<CreateInviteCommand, bool>
    {
        private readonly IInviteRepository _inviteRepository;

        public CreateInviteCommandHandler(IInviteRepository inviteRepository)
        {
            _inviteRepository = inviteRepository;
        }

        public async Task<bool> Handle(CreateInviteCommand request, CancellationToken cancellationToken)
        {
            var invite = new Domain.Aggregates.InviteAggregate.Invite(request.ServerId, request.UserId);

            _inviteRepository.Add(invite);

            return await _inviteRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
