using Huddle.Channel.Domain.Aggregates.MessageAggregate;
using MediatR;

namespace Huddle.Channel.Domain.Aggregates.Events.Messages
{
    public class MessageCreatedDomainEvent : INotification
    {
        public Guid MessageId { get; }
        public Guid ChannelId { get; }
        public Guid AuthorId { get; }
        public string Text { get; }
        public DateTime SentAt { get; }
        public bool IsEdited { get; }

        public MessageCreatedDomainEvent(Guid messageId, Guid channelId, Guid authorId, string text, DateTime sentAt, bool isEdited)
        {
            MessageId = messageId;
            ChannelId = channelId;
            AuthorId = authorId;
            Text = text;
            SentAt = sentAt;
            IsEdited = isEdited;
        }
    }
}
