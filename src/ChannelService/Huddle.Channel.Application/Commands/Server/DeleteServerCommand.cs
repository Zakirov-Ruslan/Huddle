
using MediatR;

namespace Huddle.Channel.Application.Commands.Server
{
    public class DeleteServerCommand : IRequest<bool>
    {
        public Guid CommandSenderIdenityId { get; private set; }
        public Guid ServerId { get; private set; }
        public DeleteServerCommand(Guid serverId, Guid commandSenderIdenityId)
        {
            ServerId = serverId;
            CommandSenderIdenityId = commandSenderIdenityId;
        }
    }
}
