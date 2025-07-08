using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using MediatR;

namespace Huddle.Channel.Domain.Aggregates.Events
{
    public class ChannelCreatedDomainEvent : INotification
    {
        public Guid ChannelId { get; }
        public string Name { get; }
        public ChannelType Type { get; }
        public Guid ServerId { get; }

        public ChannelCreatedDomainEvent(Guid channelId, string name, ChannelType type, Guid serverId)
        {
            ChannelId = channelId;
            Name = name;
            Type = type;
            ServerId = serverId;
        }
    }
}
