using Huddle.EventBus.Events;

namespace Huddle.Voice.WebApi.IntegrationEvents
{
    /// <summary>The LiveKit room of a voice channel is gone, so nobody is in that channel anymore.</summary>
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
