using Huddle.Channel.Domain.SeedWork;

namespace Huddle.Channel.Domain.Aggregates.InviteAggregate
{
    public class Invite : Entity, IAggregateRoot
    {
        public Guid ServerId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string Code { get; private set; }
        private Invite() { }
        public Invite(Guid serverId, string code)
        {
            ServerId = serverId;
            CreatedAt = DateTime.UtcNow;
            Code = code;
        }
    }
}
