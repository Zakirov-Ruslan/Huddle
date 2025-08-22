using Huddle.EventBus.Events;

namespace Huddle.SignalR.IntegrationEvents.Events.Voice
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
