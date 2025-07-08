using MediatR;

namespace Huddle.Channel.Domain.Aggregates.Events.Server
{
    public class ServerCreatedDomainEvent : INotification
    {
        public Guid ServerId { get; }
        public string Name { get; }
        public Guid OwnerIdentityId { get;  }

        public ServerCreatedDomainEvent(Guid serverId, string name, Guid ownerIdentityId)
        {
            ServerId = serverId;
            Name = name;
            OwnerIdentityId = ownerIdentityId;
        }
    }
}
