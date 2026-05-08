using Huddle.EventBus.Events;

namespace Huddle.SignalR.IntegrationEvents.Events.Channels
{
    public record ChannelCreatedIntegrationEvent : IntegrationEvent
    {
        public Guid ChannelId { get; }
        public Guid ServerId { get; }
        public string Name { get; }
        public string Type { get; }
        public string InitiatorSessionId { get; }

        public ChannelCreatedIntegrationEvent(Guid channelId, Guid serverId, string name, string type, string initiatorSessionId)
        {
            InitiatorSessionId = initiatorSessionId;
            ChannelId = channelId;
            ServerId = serverId;
            Name = name;
            Type = type;
        }
    }
}
