using Huddle.EventBus.Events;

namespace Huddle.SignalR.IntegrationEvents.Events.Messages
{
    public record MessageCreatedIntegrationEvent : IntegrationEvent
    {
        public Guid MessageId { get; }
        public Guid ChannelId { get; }
        public Guid AuthorId { get; }
        public string Text { get; }
        public DateTime SentAt { get; }

        public MessageCreatedIntegrationEvent(Guid messageId, Guid channelId, Guid authorId, string text, DateTime sentAt)
        {
            MessageId = messageId;
            ChannelId = channelId;
            AuthorId = authorId;
            Text = text;
            SentAt = sentAt;
        }
    }
}
