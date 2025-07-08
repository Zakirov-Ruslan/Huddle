using Huddle.Channel.Application.IntegrationEvents;
using Huddle.Channel.Application.IntegrationEvents.Events;
using Huddle.Channel.Domain.Aggregates.Events;
using MediatR;

namespace Huddle.Channel.Application.DomainEventHandlers.Channel
{
    public class CreateIntegrationEventWhenChannelDeletedDomainEventHandler : INotificationHandler<ChannelDeletedDomainEvent>
    {
        private readonly IChannelsIntegrationEventService _channelsIntegrationEventService;

        public CreateIntegrationEventWhenChannelDeletedDomainEventHandler(IChannelsIntegrationEventService channelsIntegrationEventService)
        {
            _channelsIntegrationEventService = channelsIntegrationEventService;
        }

        public async Task Handle(ChannelDeletedDomainEvent notification, CancellationToken cancellationToken)
        {
            var @event = new ChannelDeletedIntegrationEvent(notification.ChannelId, notification.ServerId);

            await _channelsIntegrationEventService.AddAndSaveEventAsync(@event);
        }
    }
}
