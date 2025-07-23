namespace Huddle.Channel.Application.Commands.Invite
{
    public class DeleteInviteCommand
    {
        public Guid InviteId { get; private set; }

        public DeleteInviteCommand(Guid inviteId)
        {
            InviteId = inviteId;
        }
    }
}
