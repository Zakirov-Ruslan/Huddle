
using MediatR;

namespace Huddle.Channel.Application.Commands.Server
{
    public class DeleteServerCommand : IRequest<bool>
    {
        public Guid ServerId { get; private set; }
        public DeleteServerCommand(Guid serverId)
        {
            ServerId = serverId;
        }
    }
}
