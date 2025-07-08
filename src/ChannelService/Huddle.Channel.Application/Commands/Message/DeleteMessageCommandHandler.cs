using Huddle.Channel.Domain.Aggregates.MessageAggregate;
using MediatR;

namespace Huddle.Channel.Application.Commands.Message
{
    public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, bool>
    {
        private readonly IMessageRepository _messageRepository;

        public DeleteMessageCommandHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<bool> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _messageRepository.GetAsync(request.MessageId)
                ?? throw new KeyNotFoundException("Message not found");

            if (message.AuthorId != request.CommandSenderId)
                return false;

            await _messageRepository.Delete(request.MessageId);
            return await _messageRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
