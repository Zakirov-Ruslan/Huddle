using MediatR;

namespace Huddle.Channel.Domain.Aggregates.Events.Server
{
    public class ServerDeletedDomainEvent : INotification
    {
        public Guid ServerId { get; set; }
        public ServerDeletedDomainEvent(Guid serverId)
        {
            ServerId = serverId;
        }
    }
}
