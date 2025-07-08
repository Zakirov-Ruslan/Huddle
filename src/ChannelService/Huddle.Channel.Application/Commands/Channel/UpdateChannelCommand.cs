using MediatR;

namespace Huddle.Channel.Application.Commands.Channel
{
    public class UpdateChannelCommand : IRequest<bool>
    {
        public Guid ServerId { get; private set; }
        public Guid Id { get; private set; }
        public string Name { get; private set; }


        public UpdateChannelCommand(Guid serverId, Guid id, string name)
        {
            ServerId = serverId;
            Id = id;
            Name = name;
        }
    }
}
