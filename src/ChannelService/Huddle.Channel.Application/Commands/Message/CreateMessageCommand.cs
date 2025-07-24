using Huddle.Channel.Application.Dto;
using MediatR;

namespace Huddle.Channel.Application.Commands.Message
{
    public class CreateMessageCommand : IRequest<MessageDto>
    {
        public Guid AuthodId { get; private set; }
        public Guid ChannelId { get; private set; }
        public string Text { get; private set; }

        public CreateMessageCommand(Guid authodId, Guid channelId, string text)
        {
            AuthodId = authodId;
            ChannelId = channelId;
            Text = text;
        }
    }
}
