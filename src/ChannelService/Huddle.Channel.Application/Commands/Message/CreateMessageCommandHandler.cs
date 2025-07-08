using Huddle.Channel.Domain.Aggregates.MessageAggregate;
using MediatR;

namespace Huddle.Channel.Application.Commands.Message
{
    public class CreateMessageCommandHandler : IRequestHandler<CreateMemberCommand, bool>
    {
        private readonly IMessageRepository _messageRepository;

        public CreateMessageCommandHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public Task<bool> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
        {
            var message = new Domain.Aggregates.MessageAggregate.Message(request.AuthodId, request.ChannelId, request.Text);

            _messageRepository.Add(message);

            return _messageRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
