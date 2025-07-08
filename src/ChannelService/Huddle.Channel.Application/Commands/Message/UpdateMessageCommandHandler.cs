using Huddle.Channel.Domain.Aggregates.MessageAggregate;
using MediatR;

namespace Huddle.Channel.Application.Commands.Message
{
    public class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommand, bool>
    {
        private readonly IMessageRepository _messageRepository;

        public UpdateMessageCommandHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<bool> Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _messageRepository.GetAsync(request.MessageId)
                ?? throw new KeyNotFoundException("Message not found");

            if (message.AuthorId != request.CommandSenderId)
                return false;

            message.EditText(request.Text);

            return await _messageRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
