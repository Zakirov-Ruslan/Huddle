using MediatR;

namespace Huddle.Channel.Application.Commands.Invite
{
    public class AcceptInviteCommand : IRequest<Guid>
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
