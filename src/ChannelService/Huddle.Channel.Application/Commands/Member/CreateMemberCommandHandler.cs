using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using MediatR;

namespace Huddle.Channel.Application.Commands.Member
{
    public class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, bool>
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IServerRepository _serverRepository;

        public CreateMemberCommandHandler(IMemberRepository memberRepository, IServerRepository serverRepository)
        {
            _memberRepository = memberRepository;
            _serverRepository = serverRepository;
        }

        public async Task<bool> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
        {
            var server = await _serverRepository.GetAsync(request.ServerId)
                ?? throw new KeyNotFoundException("Server not found");
            if (server.IsPrivate)
                throw new InvalidOperationException("Server is private");

            Domain.Aggregates.MemberAggregate.Member member = new(request.ServerId, request.IdentityId);

            _memberRepository.Add(member);

            return await _memberRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
