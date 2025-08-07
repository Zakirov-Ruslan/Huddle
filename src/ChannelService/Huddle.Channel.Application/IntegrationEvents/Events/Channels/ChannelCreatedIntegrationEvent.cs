using Huddle.EventBus.Events;

namespace Huddle.Channel.Application.IntegrationEvents.Events.Channels
{
    public record ChannelCreatedIntegrationEvent : IntegrationEvent
    {
        public Guid ChannelId { get; }
        public Guid ServerId { get; }
        public string Name { get; }
        public string Type { get; }

        public ChannelCreatedIntegrationEvent(Guid channelId, Guid serverId, string name, string type)
        {
            ChannelId = channelId;
            ServerId = serverId;
            Name = name;
            Type = type;
        }
    }
}
