using Huddle.EventBus.Events;

namespace Huddle.Voice.WebApi.IntegrationEvents
{
    /// <summary>A user joined a voice channel. <paramref name="Version"/> orders pushes against snapshots.</summary>
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
