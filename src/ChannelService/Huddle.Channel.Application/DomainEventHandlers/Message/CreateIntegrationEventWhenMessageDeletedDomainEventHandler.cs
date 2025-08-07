using Huddle.Channel.Application.IntegrationEvents.Events.Messages;
using Huddle.Channel.Application.IntegrationEvents;
using Huddle.Channel.Domain.Aggregates.Events.Messages;
using MediatR;

namespace Huddle.Channel.Application.DomainEventHandlers.Message
{
    public record CreateIntegrationEventWhenMessageDeletedDomainEventHandler : INotificationHandler<MessageDeletedDomainEvent>
    {
        private readonly IChannelsIntegrationEventService _channelsIntegrationEventService;

        public CreateIntegrationEventWhenMessageDeletedDomainEventHandler(IChannelsIntegrationEventService channelsIntegrationEventService)
        {
            _channelsIntegrationEventService = channelsIntegrationEventService;
        }

        public async Task Handle(MessageDeletedDomainEvent notification, CancellationToken cancellationToken)
        {
            var @event = new MessageDeletedIntegrationEvent(notification.ChannelId, notification.MessageId);

            await _channelsIntegrationEventService.AddAndSaveEventAsync(@event);
        }
    }
}
