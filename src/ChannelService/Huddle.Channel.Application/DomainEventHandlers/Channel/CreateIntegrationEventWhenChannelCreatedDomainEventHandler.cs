using Huddle.Channel.Application.IntegrationEvents;
using Huddle.Channel.Application.IntegrationEvents.Events;
using Huddle.Channel.Domain.Aggregates.Events;
using MediatR;

namespace Huddle.Channel.Application.DomainEventHandlers.Channel
{
    public class CreateIntegrationEventWhenChannelCreatedDomainEventHandler : INotificationHandler<ChannelCreatedDomainEvent>
    {
        private readonly IChannelsIntegrationEventService _channelsIntegrationEventService;

        public CreateIntegrationEventWhenChannelCreatedDomainEventHandler(IChannelsIntegrationEventService channelsIntegrationEventService)
        {
            _channelsIntegrationEventService = channelsIntegrationEventService;
        }

        public async Task Handle(ChannelCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var @event = new ChannelCreatedIntegrationEvent(notification.ChannelId, notification.ServerId, notification.Name, notification.Type.ToString());

            await _channelsIntegrationEventService.AddAndSaveEventAsync(@event);
        }
    }
}
