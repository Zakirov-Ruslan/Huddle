using Huddle.Channel.Application.Commands.Server;
using Huddle.Channel.Domain.Aggregates.Events.Server;
using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Huddle.Channel.Application.DomainEventHandlers.Server
{
    public class CreateMemberWhenServerCreatedEventHandler : INotificationHandler<ServerCreatedDomainEvent>
    {
        private readonly ILogger<CreateServerCommandHandler> _logger;
        private readonly IMemberRepository _memberRepository;

        public CreateMemberWhenServerCreatedEventHandler(ILogger<CreateServerCommandHandler> logger, IMemberRepository memberRepository)
        {
            _logger = logger;
            _memberRepository = memberRepository;
        }

        public async Task Handle(ServerCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            Member member = new Member(notification.ServerId, notification.OwnerIdentityId);

            _memberRepository.Add(member);
        }
    }
}
