using MediatR;

namespace Huddle.Channel.Domain.Aggregates.Events.Ivent
{
    public class InviteCreatedDomainEvent : INotification
    {
        public Guid ServerId { get; }
        public Guid UserId { get; }

        public InviteCreatedDomainEvent(Guid serverId, Guid userId)
        {
            ServerId = serverId;
            UserId = userId;
        }
    }
}
