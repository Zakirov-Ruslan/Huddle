using Huddle.EventBus.Events;

namespace Huddle.SignalR.IntegrationEvents.Events.Voice
{
    /// <summary>A user joined a voice channel. Mirrors the event published by the voice service.</summary>
    public record VoiceParticipantJoined : IntegrationEvent
    {
        public Guid ServerId { get; }
        public Guid ChannelId { get; }
        public Guid UserId { get; }
        public string DisplayName { get; }
        public DateTimeOffset JoinedAt { get; }
        public long Version { get; }

        public VoiceParticipantJoined(
            Guid serverId,
            Guid channelId,
            Guid userId,
            string displayName,
            DateTimeOffset joinedAt,
            long version)
        {
            ServerId = serverId;
            ChannelId = channelId;
            UserId = userId;
            DisplayName = displayName;
            JoinedAt = joinedAt;
            Version = version;
        }
    }
}
