using Huddle.EventBus.Events;

namespace Huddle.Channel.Application.IntegrationEvents.Events.Messages
{
    public record MessageUpdatedIntegretaionEvent : IntegrationEvent
    {
        public Guid ChannelId { get; }
        public Guid MessageId { get; }
        public string Text { get; }

        public MessageUpdatedIntegretaionEvent(Guid messageId, string text, Guid channelId)
        {
            MessageId = messageId;
            Text = text;
            ChannelId = channelId;
        }
    }
}
