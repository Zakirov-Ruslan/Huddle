using Huddle.Channel.Application.IntegrationEvents;
using Huddle.Channel.Application.IntegrationEvents.Events;
using Huddle.Channel.Domain.Aggregates.Events.Ivent;
using MediatR;

namespace Huddle.Channel.Application.DomainEventHandlers.Ivent
{
    public class CreateIntegrationEventWhenInviteCreatedDomainEventHandler : INotificationHandler<InviteCreatedDomainEvent>
    {
        private readonly IChannelsIntegrationEventService _channelsIntegrationEventService;

        public CreateIntegrationEventWhenInviteCreatedDomainEventHandler(IChannelsIntegrationEventService channelsIntegrationEventService)
        {
            _channelsIntegrationEventService = channelsIntegrationEventService;
        }

        public async Task Handle(InviteCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var @event = new InviteCreatedIntegrationEvent(notification.ServerId, notification.UserId);

            await _channelsIntegrationEventService.AddAndSaveEventAsync(@event);
        }
    }
}
