using Huddle.Channel.Application.Dto;
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
        public string InitiatorSessionId {get;}
        public IReadOnlyList<MessageAttachmentDto> Attachments { get; }

        public MessageCreatedIntegrationEvent(
            Guid messageId,
            Guid channelId,
            Guid authorId,
            string text,
            DateTime sentAt,
            string initiatorSessionId,
            IReadOnlyList<MessageAttachmentDto>? attachments = null)
        {
            MessageId = messageId;
            ChannelId = channelId;
            AuthorId = authorId;
            Text = text;
            SentAt = sentAt;
            InitiatorSessionId = initiatorSessionId;
            Attachments = attachments ?? Array.Empty<MessageAttachmentDto>();
        }
    }
}
