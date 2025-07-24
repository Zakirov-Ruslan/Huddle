using MediatR;

namespace Huddle.Channel.Application.Commands.Invite
{
    public class AcceptInviteCommand : IRequest<bool>
    {
        public Guid InviteId { get; private set; }

        public AcceptInviteCommand(Guid inviteId)
        {
            InviteId = inviteId;
        }
    }
}
