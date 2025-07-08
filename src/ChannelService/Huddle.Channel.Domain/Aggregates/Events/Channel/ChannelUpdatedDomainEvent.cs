using MediatR;

namespace Huddle.Channel.Domain.Aggregates.Events
{
    public class ChannelUpdatedDomainEvent : INotification
    {
        public Guid ChannelId { get; }
        public string Name { get; }

        public ChannelUpdatedDomainEvent(Guid channelId, string name)
        {
            ChannelId = channelId;
            Name = name;
        }
    }
}
