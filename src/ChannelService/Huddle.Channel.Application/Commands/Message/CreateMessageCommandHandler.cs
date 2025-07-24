using AutoMapper;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Exceptions;
using Huddle.Channel.Application.Services;
using Huddle.Channel.Domain.Aggregates.MessageAggregate;
using MediatR;

namespace Huddle.Channel.Application.Commands.Message
{
    public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, MessageDto>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IAccessService _accessService;

        public CreateMessageCommandHandler(IMessageRepository messageRepository, IMapper mapper, IAccessService accessService)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
            _accessService = accessService;
        }

        public async Task<MessageDto> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {
            var hasAccess = await _accessService.CanUserAccessChannelAsync(request.ChannelId, request.AuthodId);
            if (!hasAccess)
                throw new ForbiddenAccessException("User dont have access to this channel");

            var message = new Domain.Aggregates.MessageAggregate.Message(request.AuthodId, request.ChannelId, request.Text);

            _messageRepository.Add(message);

            await _messageRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return _mapper.Map<MessageDto>(message);
        }
    }
}
