using Huddle.EventBus.Events;

namespace Huddle.Voice.WebApi.IntegrationEvents
{
    public record VoiceParticipantLeft : IntegrationEvent
    {
        public Guid ServerId { get; }
        public Guid ChannelId { get; }
        public Guid UserId { get; }
        public VoiceParticipantLeft(Guid serverId, Guid channelId, Guid userId)
        {
            ServerId = serverId;
            ChannelId = channelId;
            UserId = userId;
        }
    }
}
