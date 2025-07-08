using MediatR;

namespace Huddle.Channel.Domain.Aggregates.Events
{
    public class ChannelDeletedDomainEvent : INotification
    {
        public Guid ChannelId { get; }
        public Guid ServerId { get; }
        public ChannelDeletedDomainEvent(Guid channelId, Guid serverId)
        {
            ChannelId = channelId;
            ServerId = serverId;
        }
    }
}
