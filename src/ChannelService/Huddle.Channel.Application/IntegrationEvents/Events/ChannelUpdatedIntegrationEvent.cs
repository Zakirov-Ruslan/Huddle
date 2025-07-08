using Huddle.EventBus.Events;

namespace Huddle.Channel.Application.IntegrationEvents.Events
{
    public record ChannelUpdatedIntegrationEvent : IntegrationEvent
    {
        public Guid ChannelId { get; }
        public Guid ServerId { get; }
        public string Name { get; }
        public string Type { get; }

        public ChannelUpdatedIntegrationEvent(Guid channelId, Guid serverId, string name, string type)
        {
            ChannelId = channelId;
            ServerId = serverId;
            Name = name;
            Type = type;
        }
    }
}
