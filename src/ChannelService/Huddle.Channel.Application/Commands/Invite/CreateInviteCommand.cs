using Huddle.Channel.Application.Dto;
using MediatR;

namespace Huddle.Channel.Application.Commands.Invite
{
    public class CreateInviteCommand : IRequest<InviteDto>
    {
        public Guid ServerId { get; private set; }
        public Guid? ChannelId { get; private set; }
        public Guid SenderId { get; private set; }

        public CreateInviteCommand(Guid serverId, Guid senderId)
        {
            ServerId = serverId;
            SenderId = senderId;
        }
    }
}
