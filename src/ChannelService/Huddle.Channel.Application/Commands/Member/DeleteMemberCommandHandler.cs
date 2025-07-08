using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using MediatR;

namespace Huddle.Channel.Application.Commands.Member
{
    public class DeleteMemberCommandHandler : IRequestHandler<DeleteMemberCommand, bool>
    {
        private readonly IMemberRepository _memberRepository;

        public DeleteMemberCommandHandler(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<bool> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
        {
            return true;
        }
    }
}
