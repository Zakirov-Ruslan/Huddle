using Huddle.Channel.Application.IntegrationEvents;
using Huddle.Channel.Application.IntegrationEvents.Events.Channels;
using Huddle.Channel.Domain.Aggregates.Events;
using MediatR;

namespace Huddle.Channel.Application.DomainEventHandlers.Channel
{
    public class CreateIntegrationEventWhenChannelCreatedDomainEventHandler : INotificationHandler<ChannelCreatedDomainEvent>
    {
        private readonly IChannelsIntegrationEventService _channelsIntegrationEventService;
        private readonly RequestContext _requestContext;

        public CreateIntegrationEventWhenChannelCreatedDomainEventHandler(IChannelsIntegrationEventService channelsIntegrationEventService, RequestContext requestContext)
        {
            _channelsIntegrationEventService = channelsIntegrationEventService;
            _requestContext = requestContext;
        }

        public async Task Handle(ChannelCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var @event = new ChannelCreatedIntegrationEvent(notification.ChannelId, notification.ServerId, notification.Name, notification.Type.ToString(), _requestContext.SessionId);

            await _channelsIntegrationEventService.AddAndSaveEventAsync(@event);
        }
    }
}
