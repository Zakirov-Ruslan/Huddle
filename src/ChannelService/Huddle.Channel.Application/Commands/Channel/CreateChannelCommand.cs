using Huddle.Channel.Application.Dto;
using MediatR;

namespace Huddle.Channel.Application.Commands.Channel
{
    public class CreateChannelCommand : IRequest<ChannelDto>
    {
        public Guid ServerId { get; private set; }
        public string ChannelType { get; private set; }
        public string Name { get; private set;}

        public CreateChannelCommand(Guid serverId, string channelType, string name)
        {
            ServerId = serverId;
            ChannelType = channelType;
            Name = name;
        }
    }
}
