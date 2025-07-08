using Huddle.Channel.Application.IntegrationEvents;
using Huddle.Channel.Application.IntegrationEvents.Events;
using Huddle.Channel.Domain.Aggregates.Events;
using MediatR;

namespace Huddle.Channel.Application.DomainEventHandlers.Channel
{
    public class CreateIntegrationEventWhenChannelUpdatedDomainEventHandler : INotificationHandler<ChannelCreatedDomainEvent>
    {
        private readonly IChannelsIntegrationEventService _channelsIntegrationEventService;

        public CreateIntegrationEventWhenChannelUpdatedDomainEventHandler(IChannelsIntegrationEventService channelsIntegrationEventService)
        {
            _channelsIntegrationEventService = channelsIntegrationEventService;
        }

        public async Task Handle(ChannelCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var @event = new ChannelUpdatedIntegrationEvent(notification.ChannelId, notification.ServerId, notification.Name, notification.Type.ToString());

            await _channelsIntegrationEventService.AddAndSaveEventAsync(@event);
        }
    }
}
