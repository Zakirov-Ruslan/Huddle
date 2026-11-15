namespace Huddle.Channel.Application.Dto
{
    public record MessageAttachmentDto
    (
        Guid FileId,
        string FileName,
        string ContentType,
        long Size,
        bool IsImage,
        string Url
    );

    public record MessageDto
    (
        Guid Id,
        Guid ChannelId,
        Guid AuthorId,
        string Text,
        DateTime SentAt,
        bool IsEdited,
        IReadOnlyList<MessageAttachmentDto> Attachments
    );

    public record CreateMessageRequest
    (
        string Text,
        IReadOnlyList<Guid>? AttachmentIds
    );

    public record UpdateMessageRequest
    (
        string Text
    );
}
