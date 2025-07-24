using Huddle.Channel.Application.Dto;
using MediatR;

namespace Huddle.Channel.Application.Commands.Message
{
    public class CreateMemberCommand : IRequest<MessageDto>
    {
        public Guid AuthodId { get; private set; }
        public Guid ChannelId { get; private set; }
        public string Text { get; private set; }

        public CreateMemberCommand(Guid aauthodId, Guid channelId, string text)
        {
            AuthodId = aauthodId;
            ChannelId = channelId;
            Text = text;
        }
    }
}
