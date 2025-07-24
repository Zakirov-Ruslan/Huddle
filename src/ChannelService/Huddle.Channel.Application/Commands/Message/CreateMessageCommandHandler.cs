using AutoMapper;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Domain.Aggregates.MessageAggregate;
using MediatR;

namespace Huddle.Channel.Application.Commands.Message
{
    public class CreateMessageCommandHandler : IRequestHandler<CreateMemberCommand, MessageDto>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public CreateMessageCommandHandler(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<MessageDto> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
        {
            var message = new Domain.Aggregates.MessageAggregate.Message(request.AuthodId, request.ChannelId, request.Text);

            _messageRepository.Add(message);

            await _messageRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return _mapper.Map<MessageDto>(message);
        }
    }
}
