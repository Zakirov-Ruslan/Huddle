using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using MediatR;

namespace Huddle.Channel.Application.Commands.Member
{
    public class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand, bool>
    {
        private readonly IMemberRepository _memberRepository;

        public UpdateMemberCommandHandler(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<bool> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
        {
            var member = await _memberRepository.GetAsync(request.MemberId)
                ?? throw new KeyNotFoundException("Member not found");

            // check admin rights also
            if (request.CommandSenderId != member.Id)
                return false;

            member.Profile.ChangeServerUsername(request.ServerUsername);

            return await _memberRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
