using Huddle.Channel.Application.IntegrationEvents;
using Huddle.Channel.Domain.Aggregates.Events.Messages;
using MediatR;
using Huddle.Channel.Application.IntegrationEvents.Events.Messages;

namespace Huddle.Channel.Application.DomainEventHandlers.Message
{
    public class CreateIntegrationEventWhenMessageCreatedDomainEventHandler : INotificationHandler<MessageCreatedDomainEvent>
    {
        private readonly IChannelsIntegrationEventService _channelsIntegrationEventService;

        public CreateIntegrationEventWhenMessageCreatedDomainEventHandler(IChannelsIntegrationEventService channelsIntegrationEventService)
        {
            _channelsIntegrationEventService = channelsIntegrationEventService;
        }

        public async Task Handle(MessageCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var @event = new MessageCreatedIntegrationEvent(notification.MessageId, notification.ChannelId, notification.AuthorId, notification.Text, notification.SentAt);

            await _channelsIntegrationEventService.AddAndSaveEventAsync(@event);
        }
    }
}
