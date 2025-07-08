using MediatR;

namespace Huddle.Channel.Application.Commands.Message
{
    public class UpdateMessageCommand : IRequest<bool>
    {
        public Guid MessageId { get; private set; }
        public Guid CommandSenderId { get; private set; }
        public string Text { get; private set; }

        public UpdateMessageCommand(Guid messageId, Guid commandSenderId, string text)
        {
            MessageId = messageId;
            CommandSenderId = commandSenderId;
            Text = text;
        }
    }
}
