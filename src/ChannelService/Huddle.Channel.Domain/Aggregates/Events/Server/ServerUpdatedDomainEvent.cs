using MediatR;

namespace Huddle.Channel.Domain.Aggregates.Events.Server
{
    public class ServerUpdatedDomainEvent : INotification
    {
        public Guid Id { get;  }
        public string Name { get; }

        public ServerUpdatedDomainEvent(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
