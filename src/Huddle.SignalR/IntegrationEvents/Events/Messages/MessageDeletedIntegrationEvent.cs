using Huddle.EventBus.Events;

namespace Huddle.SignalR.IntegrationEvents.Events.Messages
{
    public record MessageDeletedIntegrationEvent : IntegrationEvent
    {
        public Guid ChannelId { get; }
        public Guid MessageId { get; }

        public MessageDeletedIntegrationEvent(Guid channelId, Guid messageId)
        {
            ChannelId = channelId;
            MessageId = messageId;
        }
    }
}
