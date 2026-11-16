using Huddle.Channel.Domain.Aggregates.Events.Messages;
using Huddle.Channel.Domain.Aggregates.MessageAggregate;

namespace Huddle.Channel.Domain.UnitTests.Domain;

public class MessageTests
{
    private static readonly Guid AuthorId = Guid.NewGuid();
    private static readonly Guid ChannelId = Guid.NewGuid();

    [Fact]
    public void Constructor_SetsProperties()
    {
        var before = DateTime.UtcNow;

        var message = new Message(AuthorId, ChannelId, "hello");

        Assert.NotEqual(Guid.Empty, message.Id);
        Assert.Equal(AuthorId, message.AuthorId);
        Assert.Equal(ChannelId, message.ChannelId);
        Assert.Equal("hello", message.Text);
        Assert.False(message.IsEdited);
        Assert.InRange(message.SentAt, before, DateTime.UtcNow);
        Assert.Empty(message.Attachments);
    }

    [Fact]
    public void Constructor_UsesUuidV7Ids()
    {
        var message = new Message(AuthorId, ChannelId, "first");

        // Cursor pagination compares ids, so they have to be time based (RFC 9562 version 7).
        Assert.Equal('7', message.Id.ToString("D")[14]);
    }

    [Fact]
    public void Constructor_WithAttachments_KeepsThemAndRaisesDomainEvent()
    {
        var attachment = new MessageAttachment(Guid.NewGuid(), "photo.png", "image/png", 1024, true, 0);

        var message = new Message(AuthorId, ChannelId, "hello", [attachment]);

        Assert.Same(attachment, Assert.Single(message.Attachments));
        Assert.NotNull(message.DomainEvents);
        var domainEvent = Assert.IsType<MessageCreatedDomainEvent>(Assert.Single(message.DomainEvents));
        Assert.Equal(message.Id, domainEvent.MessageId);
        Assert.Equal(ChannelId, domainEvent.ChannelId);
        Assert.Equal(AuthorId, domainEvent.AuthorId);
        Assert.Equal("hello", domainEvent.Text);
        Assert.Equal(message.SentAt, domainEvent.SentAt);
        Assert.False(domainEvent.IsEdited);
        Assert.Same(attachment, Assert.Single(domainEvent.Attachments));
    }

    [Fact]
    public void Constructor_WithoutAttachments_RaisesDomainEventWithEmptyAttachments()
    {
        var message = new Message(AuthorId, ChannelId, "hello");

        Assert.NotNull(message.DomainEvents);
        var domainEvent = Assert.IsType<MessageCreatedDomainEvent>(Assert.Single(message.DomainEvents));
        Assert.Empty(domainEvent.Attachments);
    }

    [Fact]
    public void EditText_MarksMessageAsEditedAndRaisesDomainEvent()
    {
        var message = new Message(AuthorId, ChannelId, "hello");
        message.ClearDomainEvents();

        message.EditText("hello, world");

        Assert.Equal("hello, world", message.Text);
        Assert.True(message.IsEdited);
        Assert.NotNull(message.DomainEvents);
        var domainEvent = Assert.IsType<MessageUpdatedDomainEvent>(Assert.Single(message.DomainEvents));
        Assert.Equal(message.Id, domainEvent.Id);
        Assert.Equal(ChannelId, domainEvent.ChannelId);
        Assert.Equal("hello, world", domainEvent.Text);
    }

    [Fact]
    public void MarkToDelete_RaisesDomainEvent()
    {
        var message = new Message(AuthorId, ChannelId, "hello");
        message.ClearDomainEvents();

        message.MarkToDelete();

        Assert.NotNull(message.DomainEvents);
        var domainEvent = Assert.IsType<MessageDeletedDomainEvent>(Assert.Single(message.DomainEvents));
        Assert.Equal(message.Id, domainEvent.MessageId);
        Assert.Equal(ChannelId, domainEvent.ChannelId);
    }
}
