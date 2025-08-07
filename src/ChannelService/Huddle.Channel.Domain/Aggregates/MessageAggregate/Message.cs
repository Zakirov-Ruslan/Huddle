using Huddle.Channel.Domain.Aggregates.Events.Messages;
using Huddle.Channel.Domain.SeedWork;

namespace Huddle.Channel.Domain.Aggregates.MessageAggregate
{
    public class Message : Entity, IAggregateRoot
    {
        public Guid ChannelId { get; private set; }
        public Guid AuthorId { get; private set; }
        public string Text { get; private set; }
        public DateTime SentAt { get; private set; }
        public bool IsEdited { get; private set; }

        private Message() { }
        public Message(Guid authorId, Guid channelId, string text)
        {
            Id = Guid.NewGuid();
            AuthorId = authorId;
            ChannelId = channelId;
            Text = text;
            SentAt = DateTime.UtcNow;
            IsEdited = false;

            MessageCreatedDomainEvent @event = new(Id, ChannelId, AuthorId, Text, SentAt, IsEdited);
            AddDomainEvent(@event);
        }

        public void EditText(string text)
        {
            Text = text;
            IsEdited = true;

            MessageUpdatedDomainEvent @event = new(Id, ChannelId, text);
            AddDomainEvent(@event);
        }

        public void MarkToDelete()
        {
            MessageDeletedDomainEvent @event = new(ChannelId, Id);
            AddDomainEvent(@event);
        }
    }
}
