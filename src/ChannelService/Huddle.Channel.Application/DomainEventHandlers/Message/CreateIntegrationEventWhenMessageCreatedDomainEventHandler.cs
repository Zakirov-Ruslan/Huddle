using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.IntegrationEvents;
using Huddle.Channel.Application.Services;
using Huddle.Channel.Domain.Aggregates.Events.Messages;
using MediatR;
using Huddle.Channel.Application.IntegrationEvents.Events.Messages;

namespace Huddle.Channel.Application.DomainEventHandlers.Message
{
    public class CreateIntegrationEventWhenMessageCreatedDomainEventHandler : INotificationHandler<MessageCreatedDomainEvent>
    {
        private readonly IChannelsIntegrationEventService _channelsIntegrationEventService;
        private readonly RequestContext _requestContext;
        private readonly IFileUrlProvider _fileUrlProvider;

        public CreateIntegrationEventWhenMessageCreatedDomainEventHandler(
            IChannelsIntegrationEventService channelsIntegrationEventService,
            RequestContext requestContext,
            IFileUrlProvider fileUrlProvider)
        {
            _channelsIntegrationEventService = channelsIntegrationEventService;
            _requestContext = requestContext;
            _fileUrlProvider = fileUrlProvider;
        }

        public async Task Handle(MessageCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var attachments = notification.Attachments
                .OrderBy(attachment => attachment.Position)
                .Select(attachment => new MessageAttachmentDto(
                    attachment.FileId,
                    attachment.FileName,
                    attachment.ContentType,
                    attachment.Size,
                    attachment.IsImage,
                    _fileUrlProvider.BuildUrl(attachment.FileId)))
                .ToList();

            var @event = new MessageCreatedIntegrationEvent(
                notification.MessageId,
                notification.ChannelId,
                notification.AuthorId,
                notification.Text,
                notification.SentAt,
                _requestContext.SessionId,
                attachments);

            await _channelsIntegrationEventService.AddAndSaveEventAsync(@event);
        }
    }
}
