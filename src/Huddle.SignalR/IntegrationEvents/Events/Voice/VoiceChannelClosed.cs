using Huddle.EventBus.Events;

namespace Huddle.SignalR.IntegrationEvents.Events.Voice
{
    /// <summary>The voice channel room is gone. Mirrors the event published by the voice service.</summary>
    public record VoiceChannelClosed : IntegrationEvent
    {
        public Guid ServerId { get; }
        public Guid ChannelId { get; }
        public long Version { get; }

        public VoiceChannelClosed(Guid serverId, Guid channelId, long version)
        {
            ServerId = serverId;
            ChannelId = channelId;
            Version = version;
        }
    }
}
