using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Exceptions;
using Huddle.Channel.Application.Mapping;
using Huddle.Channel.Application.Services;
using Huddle.Channel.Domain.Aggregates.MessageAggregate;
using MediatR;

namespace Huddle.Channel.Application.Commands.Message
{
    public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, MessageDto>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMessageDtoFactory _messageDtoFactory;
        private readonly IAccessService _accessService;
        private readonly IFilesClient _filesClient;

        public CreateMessageCommandHandler(
            IMessageRepository messageRepository,
            IMessageDtoFactory messageDtoFactory,
            IAccessService accessService,
            IFilesClient filesClient)
        {
            _messageRepository = messageRepository;
            _messageDtoFactory = messageDtoFactory;
            _accessService = accessService;
            _filesClient = filesClient;
        }

        public async Task<MessageDto> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {
            var hasAccess = await _accessService.CanUserAccessChannelAsync(request.ChannelId, request.AuthodId);
            if (!hasAccess)
                throw new ForbiddenAccessException("User dont have access to this channel");

            var attachments = await ClaimAttachmentsAsync(request, cancellationToken);

            var message = new Domain.Aggregates.MessageAggregate.Message(request.AuthodId, request.ChannelId, request.Text, attachments);

            _messageRepository.Add(message);

            await _messageRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return _messageDtoFactory.Create(message);
        }

        private async Task<IReadOnlyList<MessageAttachment>> ClaimAttachmentsAsync(CreateMessageCommand request, CancellationToken cancellationToken)
        {
            if (request.AttachmentIds.Count == 0)
                return [];

            var claimedFiles = await _filesClient.ClaimAttachmentsAsync(request.AuthodId, request.AttachmentIds, cancellationToken);

            return claimedFiles
                .Select((file, index) => new MessageAttachment(file.Id, file.FileName, file.ContentType, file.Size, file.IsImage, index))
                .ToList();
        }
    }
}
