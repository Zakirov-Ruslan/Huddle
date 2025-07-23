namespace Huddle.Channel.Application.Commands.Invite
{
    public class CreateInviteCommand
    {
        public Guid ServerId { get; private set; }
        public Guid UserId { get; private set; }

        public CreateInviteCommand(Guid serverId, Guid userId)
        {
            ServerId = serverId;
            UserId = userId;
        }
    }
}
