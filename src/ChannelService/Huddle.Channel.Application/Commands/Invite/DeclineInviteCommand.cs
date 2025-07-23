namespace Huddle.Channel.Application.Commands.Invite
{
    public class DeclineInviteCommand
    {
        public Guid InviteId { get; private set; }

        public DeclineInviteCommand(Guid inviteId)
        {
            InviteId = inviteId;
        }
    }
}
