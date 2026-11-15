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

        private readonly List<MessageAttachment> _attachments = [];
        public IEnumerable<MessageAttachment> Attachments => _attachments.AsReadOnly();

        private Message() { }

        public Message(Guid authorId, Guid channelId, string text, IEnumerable<MessageAttachment>? attachments = null)
        {
            Id = Guid.CreateVersion7();
            AuthorId = authorId;
            ChannelId = channelId;
            Text = text;
            SentAt = DateTime.UtcNow;
            IsEdited = false;

            if (attachments is not null)
                _attachments.AddRange(attachments);

            MessageCreatedDomainEvent @event = new(Id, ChannelId, AuthorId, Text, SentAt, IsEdited, _attachments.AsReadOnly());
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
