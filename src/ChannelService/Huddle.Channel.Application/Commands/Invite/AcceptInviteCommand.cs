using MediatR;

namespace Huddle.Channel.Application.Commands.Invite
{
    public class AcceptInviteCommand : IRequest<bool>
    {
        public string InviteCode { get; private set; }
        public Guid IdentityId { get; private set; }

        public AcceptInviteCommand(string inviteCode, Guid identityId)
        {
            InviteCode = inviteCode;
            IdentityId = identityId;
        }
    }
}
