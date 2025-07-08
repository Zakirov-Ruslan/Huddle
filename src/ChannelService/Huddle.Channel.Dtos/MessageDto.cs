namespace Huddle.Channel.Application.Dto
{
    public record MessageDto
    (
        Guid Id,
        Guid ChannelId,
        Guid AuthorId,
        string Text,
        DateTime SentAt,
        bool IsEdited
    );

    public record CreateMessageRequest
    (
        Guid AuthorId,
        string Text
    );

    public record UpdateMessageRequest
    (
        Guid MessageId,
        string Text
    );
}
