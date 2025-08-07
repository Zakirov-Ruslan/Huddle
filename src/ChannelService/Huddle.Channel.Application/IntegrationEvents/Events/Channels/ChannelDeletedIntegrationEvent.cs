using Huddle.EventBus.Events;

namespace Huddle.Channel.Application.IntegrationEvents.Events.Channels
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
