using MediatR;

namespace Huddle.Channel.Application.Commands.Message
{
    public class DeleteMessageCommand : IRequest<bool>
    {
        public Guid MessageId { get; private set; }
        public Guid CommandSenderId { get; private set; }

        public DeleteMessageCommand(Guid messageId, Guid commandSenderId)
        {
            MessageId = messageId;
            CommandSenderId = commandSenderId;
        }
    }
}
