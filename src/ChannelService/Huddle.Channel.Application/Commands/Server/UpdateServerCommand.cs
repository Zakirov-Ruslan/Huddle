using MediatR;

namespace Huddle.Channel.Application.Commands.Server
{
    public  class UpdateServerCommand : IRequest<bool>
    {
        public Guid CommandSenderIdenityId { get; private set; }
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        /// <summary>New avatar uploaded by the user, ignored when <see cref="RemoveAvatar"/> is set.</summary>
        public Guid? AvatarFileId { get; private set; }
        public bool RemoveAvatar { get; private set; }

        public UpdateServerCommand(Guid id, string name, Guid commandSenderIdenityId, Guid? avatarFileId = null, bool removeAvatar = false)
        {
            Id = id;
            Name = name;
            CommandSenderIdenityId = commandSenderIdenityId;
            AvatarFileId = avatarFileId;
            RemoveAvatar = removeAvatar;
        }
    }
}
