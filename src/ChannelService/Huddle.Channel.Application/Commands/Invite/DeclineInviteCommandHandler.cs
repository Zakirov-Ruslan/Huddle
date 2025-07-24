using Huddle.Channel.Domain.Aggregates.InviteAggregate;
using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using MediatR;

namespace Huddle.Channel.Application.Commands.Invite
{
    public  class DeclineInviteCommandHandler : IRequestHandler<DeclineInviteCommand, bool>
    {
        private readonly IInviteRepository _inviteRepository;
        private readonly IMemberRepository _memberRepository;

        public DeclineInviteCommandHandler(IInviteRepository inviteRepository, IMemberRepository memberRepository)
        {
            _inviteRepository = inviteRepository;
            _memberRepository = memberRepository;
        }

        public async Task<bool> Handle(DeclineInviteCommand request, CancellationToken cancellationToken)
        {
            await _inviteRepository.Delete(request.InviteId);

            return await _memberRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
