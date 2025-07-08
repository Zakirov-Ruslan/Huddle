using MediatR;

namespace Huddle.Channel.Application.Commands.Member
{
    public  class UpdateMemberCommand : IRequest<bool>
    {
        public Guid ServerId { get; private set; }
        public Guid MemberId { get; private set; }
        public Guid CommandSenderId { get; private set; }
        public string ServerUsername { get; private set; }

        public UpdateMemberCommand(Guid memberId, string serverUsername, Guid commandSenderId, Guid serverId)
        {
            MemberId = memberId;
            ServerUsername = serverUsername;
            CommandSenderId = commandSenderId;
            ServerId = serverId;
        }
    }
}
