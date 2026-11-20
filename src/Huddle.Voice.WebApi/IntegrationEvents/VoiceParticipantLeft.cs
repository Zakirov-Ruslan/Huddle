using Huddle.EventBus.Events;

namespace Huddle.Voice.WebApi.IntegrationEvents
{
    /// <summary>
    /// A user left a voice channel. <paramref name="Reason"/> is either
    /// <c>participant_left</c> or <c>participant_connection_aborted</c> (the LiveKit webhook that caused it).
    /// </summary>
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
