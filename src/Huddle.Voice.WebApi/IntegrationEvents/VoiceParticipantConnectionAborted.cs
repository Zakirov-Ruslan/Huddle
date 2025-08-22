using Huddle.EventBus.Events;

namespace Huddle.Voice.WebApi.IntegrationEvents
{
    public record VoiceParticipantConnectionAborted : IntegrationEvent
    {
        public Guid ServerId { get; }
        public Guid ChannelId { get; }
        public Guid UserId { get; }

        public VoiceParticipantConnectionAborted(Guid serverId, Guid channelId, Guid userId)
        {
            ServerId = serverId;
            ChannelId = channelId;
            UserId = userId;
        }
    }
}