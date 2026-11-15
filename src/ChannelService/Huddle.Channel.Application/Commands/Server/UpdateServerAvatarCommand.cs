using MediatR;

namespace Huddle.Channel.Application.Commands.Server
{
    public class UpdateServerAvatarCommand : IRequest<bool>
    {
        public Guid ServerId { get; private set; }
        public Guid AvatarFileId { get; private set; }
        public Guid CommandSenderIdentityId { get; private set; }

        public UpdateServerAvatarCommand(Guid serverId, Guid avatarFileId, Guid commandSenderIdentityId)
        {
            ServerId = serverId;
            AvatarFileId = avatarFileId;
            CommandSenderIdentityId = commandSenderIdentityId;
        }
    }
}
