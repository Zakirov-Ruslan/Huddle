using Huddle.Channel.Domain.Aggregates.Events.Ivent;
using Huddle.Channel.Domain.SeedWork;

namespace Huddle.Channel.Domain.Aggregates.InviteAggregate
{
    // Its just personal invite model. Need to create another one for a public link with some code like this https://discord.gg/ptFxAC9Y
    public class Invite : Entity, IAggregateRoot
    {
        public Guid ServerId { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        
        //public string Code { get; private set; }
        private Invite() { }
        public Invite(Guid serverId, Guid userId)
        {
            ServerId = serverId;
            UserId = userId;

            

            InviteCreatedDomainEvent @event = new(ServerId, userId);
            AddDomainEvent(@event);
        }
    }
}
