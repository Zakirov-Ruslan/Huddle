using Huddle.Channel.Domain.Aggregates.MessageAggregate;
using MediatR;

namespace Huddle.Channel.Domain.Aggregates.Events.Messages
{
    public class MessageUpdatedDomainEvent : INotification
    {
        public Guid Id { get; }
        public Guid ChannelId { get; }
        public string Text { get; }

        public MessageUpdatedDomainEvent(Guid id, Guid channelId, string text)
        {
            Text = text;
            Id = id;
            ChannelId = channelId;
        }
    }
}
