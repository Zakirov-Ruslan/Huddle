using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using MediatR;

namespace Huddle.Channel.Application.Commands.Member
{
    public class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, bool>
    {
        private readonly IMemberRepository _memberRepository;

        public CreateMemberCommandHandler(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<bool> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
        {
            Domain.Aggregates.MemberAggregate.Member member = new(request.ServerId, request.IdentityId);

            _memberRepository.Add(member);

            return await _memberRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
