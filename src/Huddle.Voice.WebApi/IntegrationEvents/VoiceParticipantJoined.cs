using Huddle.EventBus.Events;

namespace Huddle.Voice.WebApi.IntegrationEvents
{
    public record VoiceParticipantJoined : IntegrationEvent
    {
        public Guid ServerId { get; }
        public Guid ChannelId { get; }
        public Guid UserId { get; }
        public VoiceParticipantJoined(Guid serverId, Guid channelId, Guid userId)
        {
            ServerId = serverId;
            ChannelId = channelId;
            UserId = userId;
        }
    }
}
