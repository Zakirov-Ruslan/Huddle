using Huddle.Channel.Domain.Aggregates.InviteAggregate;
using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using MediatR;

namespace Huddle.Channel.Application.Commands.Invite
{
    public class AcceptInviteCommandHandler : IRequestHandler<AcceptInviteCommand, bool>
    {
        private readonly IInviteRepository _inviteRepository;
        private readonly IMemberRepository _memberRepository;

        public AcceptInviteCommandHandler(IInviteRepository inviteRepository, IMemberRepository memberRepository)
        {
            _inviteRepository = inviteRepository;
            _memberRepository = memberRepository;
        }

        public async Task<bool> Handle(AcceptInviteCommand request, CancellationToken cancellationToken)
        {
            var invite = await _inviteRepository.GetByCode(request.InviteCode)
                ?? throw new KeyNotFoundException("Invite not found");

            Domain.Aggregates.MemberAggregate.Member member = new(invite.ServerId, request.IdentityId);
            _memberRepository.Add(member);

            var result = await _memberRepository.UnitOfWork.SaveEntitiesAsync();

            return result;
        }
    }
}
