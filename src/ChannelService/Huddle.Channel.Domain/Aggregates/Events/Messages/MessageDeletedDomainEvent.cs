using MediatR;

namespace Huddle.Channel.Domain.Aggregates.Events.Messages
{
    public class MessageDeletedDomainEvent : INotification
    {
        public Guid ChannelId { get; }
        public Guid MessageId { get; }

        public MessageDeletedDomainEvent(Guid channelId, Guid messageId)
        {
            ChannelId = channelId;
            MessageId = messageId;
        }
    }
}
