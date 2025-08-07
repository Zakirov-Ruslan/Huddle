using Huddle.Channel.Application.IntegrationEvents.Events.Messages;
using Huddle.Channel.Application.IntegrationEvents;
using Huddle.Channel.Domain.Aggregates.Events.Messages;
using MediatR;

namespace Huddle.Channel.Application.DomainEventHandlers.Message
{
    public class CreateIntegrationEventWhenMessageUpdatedDomainEventHandler : INotificationHandler<MessageUpdatedDomainEvent>
    {
        private readonly IChannelsIntegrationEventService _channelsIntegrationEventService;

        public CreateIntegrationEventWhenMessageUpdatedDomainEventHandler(IChannelsIntegrationEventService channelsIntegrationEventService)
        {
            _channelsIntegrationEventService = channelsIntegrationEventService;
        }

        public async Task Handle(MessageUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var @event = new MessageUpdatedIntegretaionEvent(notification.Id, notification.Text, notification.ChannelId);

            await _channelsIntegrationEventService.AddAndSaveEventAsync(@event);
        }
    }
}
