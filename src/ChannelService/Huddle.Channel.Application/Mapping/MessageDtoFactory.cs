using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Services;
using Huddle.Channel.Domain.Aggregates.MessageAggregate;

namespace Huddle.Channel.Application.Mapping
{
    public interface IMessageDtoFactory
    {
        MessageDto Create(Message message);
    }

    public class MessageDtoFactory : IMessageDtoFactory
    {
        private readonly IFileUrlProvider _fileUrlProvider;

        public MessageDtoFactory(IFileUrlProvider fileUrlProvider)
        {
            _fileUrlProvider = fileUrlProvider;
        }

        public MessageDto Create(Message message) =>
            new(
                message.Id,
                message.ChannelId,
                message.AuthorId,
                message.Text,
                message.SentAt,
                message.IsEdited,
                message.Attachments
                    .OrderBy(attachment => attachment.Position)
                    .Select(attachment => new MessageAttachmentDto(
                        attachment.FileId,
                        attachment.FileName,
                        attachment.ContentType,
                        attachment.Size,
                        attachment.IsImage,
                        _fileUrlProvider.BuildUrl(attachment.FileId)))
                    .ToList());
    }
}
