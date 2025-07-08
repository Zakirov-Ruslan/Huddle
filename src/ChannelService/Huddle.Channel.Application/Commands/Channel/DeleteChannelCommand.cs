using MediatR;

namespace Huddle.Channel.Application.Commands.Channel
{
    public class DeleteChannelCommand : IRequest<bool>
    {
        public Guid ServerId { get; private set; }
        public Guid Id { get; private set; }

        public DeleteChannelCommand(Guid serverId, Guid id)
        {
            ServerId = serverId;
            Id = id;
        }
    }
}
