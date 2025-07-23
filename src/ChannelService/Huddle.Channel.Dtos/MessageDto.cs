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
        string Text
    );

    public record UpdateMessageRequest
    (
        string Text
    );
}
