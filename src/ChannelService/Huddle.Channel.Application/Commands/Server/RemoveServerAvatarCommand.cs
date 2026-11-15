using MediatR;

namespace Huddle.Channel.Application.Commands.Server
{
    public class RemoveServerAvatarCommand : IRequest<bool>
    {
        public Guid ServerId { get; private set; }
        public Guid CommandSenderIdentityId { get; private set; }

        public RemoveServerAvatarCommand(Guid serverId, Guid commandSenderIdentityId)
        {
            ServerId = serverId;
            CommandSenderIdentityId = commandSenderIdentityId;
        }
    }
}
