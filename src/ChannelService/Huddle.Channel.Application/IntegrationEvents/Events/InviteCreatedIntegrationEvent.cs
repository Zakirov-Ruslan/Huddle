using Huddle.EventBus.Events;

namespace Huddle.Channel.Application.IntegrationEvents.Events
{
    public record InviteCreatedIntegrationEvent : IntegrationEvent
    {
        public Guid ServerId { get;  }
        public Guid IdentityId { get; }

        public InviteCreatedIntegrationEvent(Guid serverId, Guid identityId)
        {
            ServerId = serverId;
            IdentityId = identityId;
        }
    }
}
