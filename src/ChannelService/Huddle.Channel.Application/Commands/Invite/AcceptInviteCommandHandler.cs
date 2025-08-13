using Huddle.Channel.Domain.Aggregates.InviteAggregate;
using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using MediatR;

namespace Huddle.Channel.Application.Commands.Invite
{
    public class AcceptInviteCommandHandler : IRequestHandler<AcceptInviteCommand, Guid>
    {
        private readonly IInviteRepository _inviteRepository;
        private readonly IMemberRepository _memberRepository;

        public AcceptInviteCommandHandler(IInviteRepository inviteRepository, IMemberRepository memberRepository)
        {
            _inviteRepository = inviteRepository;
            _memberRepository = memberRepository;
        }

        public async Task<Guid> Handle(AcceptInviteCommand request, CancellationToken cancellationToken)
        {
            var invite = await _inviteRepository.GetByCode(request.InviteCode)
                ?? throw new KeyNotFoundException("Invite not found");

            var alreadyMember = await _memberRepository.GetByServerAndIdentityIdAsync(invite.ServerId, request.IdentityId) is not null;
            if (alreadyMember)
                return invite.ServerId;

            Domain.Aggregates.MemberAggregate.Member member = new(invite.ServerId, request.IdentityId);
            _memberRepository.Add(member);

            var result = await _memberRepository.UnitOfWork.SaveEntitiesAsync();

            return invite.ServerId;
        }
    }
}
