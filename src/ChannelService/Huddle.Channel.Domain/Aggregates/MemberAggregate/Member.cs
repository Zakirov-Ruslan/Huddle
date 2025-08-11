using Huddle.Channel.Domain.SeedWork;

namespace Huddle.Channel.Domain.Aggregates.MemberAggregate
{
    public class Member : Entity, IAggregateRoot
    {
        public Guid IdentityId { get; private set; }
        public Guid ServerId { get; private set; }
        public MemberProfile Profile { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Member() { }

        public Member(Guid serverId, Guid identityId)
        {
            IdentityId = identityId;
            ServerId = serverId;
            CreatedAt = DateTime.UtcNow;

            Profile = new();
        }
    }
}
