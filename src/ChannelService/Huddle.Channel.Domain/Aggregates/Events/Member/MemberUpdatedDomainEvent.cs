using MediatR;

namespace Huddle.Channel.Domain.Aggregates.Events.Member
{
    public class MemberUpdatedDomainEvent : INotification
    {
        public Guid MemberId { get; }
        public string Username { get; }
        public string Description  { get; }
        public MemberUpdatedDomainEvent(Guid memberId, string username, string description)
        {
            MemberId = memberId;
            Username = username;
            Description = description;
        }
    }
}
