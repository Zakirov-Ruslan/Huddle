using Huddle.EventBus.Events;

namespace Huddle.SignalR.IntegrationEvents.Events.Messages
{
    public record MessageUpdatedIntegrationEvent : IntegrationEvent
    {
        public Guid ChannelId { get; }
        public Guid MessageId { get; }
        public string Text { get; }

        public MessageUpdatedIntegrationEvent(Guid messageId, string text, Guid channelId)
        {
            MessageId = messageId;
            Text = text;
            ChannelId = channelId;
        }
    }
}
