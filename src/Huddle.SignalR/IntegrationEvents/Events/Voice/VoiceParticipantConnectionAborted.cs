using Huddle.EventBus.Events;

namespace Huddle.SignalR.IntegrationEvents.Events.Voice
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