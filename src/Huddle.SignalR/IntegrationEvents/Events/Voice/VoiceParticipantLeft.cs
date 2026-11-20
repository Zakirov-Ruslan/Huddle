using Huddle.EventBus.Events;

namespace Huddle.SignalR.IntegrationEvents.Events.Voice
{
    /// <summary>A user left a voice channel. Mirrors the event published by the voice service.</summary>
    public record VoiceParticipantLeft : IntegrationEvent
    {
        public Guid ServerId { get; }
        public Guid ChannelId { get; }
        public Guid UserId { get; }
        public string Reason { get; }
        public long Version { get; }

        public VoiceParticipantLeft(Guid serverId, Guid channelId, Guid userId, string reason, long version)
        {
            ServerId = serverId;
            ChannelId = channelId;
            UserId = userId;
            Reason = reason;
            Version = version;
        }
    }
}
