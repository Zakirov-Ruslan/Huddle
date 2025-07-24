using MediatR;

namespace Huddle.Channel.Application.Commands.Invite
{
    public class DeclineInviteCommand : IRequest<bool>
    {
        public Guid InviteId { get; private set; }

        public DeclineInviteCommand(Guid inviteId)
        {
            InviteId = inviteId;
        }
    }
}
