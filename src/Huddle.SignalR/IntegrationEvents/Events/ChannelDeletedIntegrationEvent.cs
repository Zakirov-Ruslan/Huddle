using Huddle.EventBus.Events;

namespace Huddle.SignalR.IntegrationEvents.Events
{
    public record ChannelDeletedIntegrationEvent : IntegrationEvent
    {
        public Guid ChannelId { get; }
        public Guid ServerId { get; }

        public ChannelDeletedIntegrationEvent(Guid channelId, Guid serverId)
        {
            ChannelId = channelId;
            ServerId = serverId;
        }
    }
}
