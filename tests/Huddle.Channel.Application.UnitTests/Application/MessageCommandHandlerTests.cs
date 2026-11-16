using Huddle.Channel.Application.Commands.Message;
using Huddle.Channel.Domain.Aggregates.MessageAggregate;
using Huddle.Channel.Domain.SeedWork;
using NSubstitute;

namespace Huddle.Channel.Application.UnitTests.Application;

public class UpdateMessageCommandHandlerTests
{
    private static readonly Guid AuthorId = Guid.NewGuid();
    private static readonly Guid ChannelId = Guid.NewGuid();

    private readonly IMessageRepository _messageRepository = Substitute.For<IMessageRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private UpdateMessageCommandHandler CreateHandler()
    {
        _messageRepository.UnitOfWork.Returns(_unitOfWork);
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);
        return new UpdateMessageCommandHandler(_messageRepository);
    }

    [Fact]
    public async Task Handle_WithUnknownMessage_Throws()
    {
        var handler = CreateHandler();
        _messageRepository.GetAsync(Arg.Any<Guid>()).Returns((Message?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(
            new UpdateMessageCommand(Guid.NewGuid(), AuthorId, "new text"),
            CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenSenderIsNotAuthor_ReturnsFalseWithoutSaving()
    {
        var handler = CreateHandler();
        var message = new Message(AuthorId, ChannelId, "hello");
        _messageRepository.GetAsync(message.Id).Returns(message);

        var result = await handler.Handle(new UpdateMessageCommand(message.Id, Guid.NewGuid(), "new text"), CancellationToken.None);

        Assert.False(result);
        Assert.Equal("hello", message.Text);
        Assert.False(message.IsEdited);
        await _unitOfWork.DidNotReceive().SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSenderIsAuthor_EditsAndSavesMessage()
    {
        var handler = CreateHandler();
        var message = new Message(AuthorId, ChannelId, "hello");
        _messageRepository.GetAsync(message.Id).Returns(message);

        var result = await handler.Handle(new UpdateMessageCommand(message.Id, AuthorId, "new text"), CancellationToken.None);

        Assert.True(result);
        Assert.Equal("new text", message.Text);
        Assert.True(message.IsEdited);
        await _unitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }
}

public class DeleteMessageCommandHandlerTests
{
    private static readonly Guid AuthorId = Guid.NewGuid();
    private static readonly Guid ChannelId = Guid.NewGuid();

    private readonly IMessageRepository _messageRepository = Substitute.For<IMessageRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private DeleteMessageCommandHandler CreateHandler()
    {
        _messageRepository.UnitOfWork.Returns(_unitOfWork);
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);
        return new DeleteMessageCommandHandler(_messageRepository);
    }

    [Fact]
    public async Task Handle_WithUnknownMessage_Throws()
    {
        var handler = CreateHandler();
        _messageRepository.GetAsync(Arg.Any<Guid>()).Returns((Message?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(
            new DeleteMessageCommand(Guid.NewGuid(), AuthorId),
            CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenSenderIsNotAuthor_ReturnsFalseWithoutDeleting()
    {
        var handler = CreateHandler();
        var message = new Message(AuthorId, ChannelId, "hello");
        _messageRepository.GetAsync(message.Id).Returns(message);

        var result = await handler.Handle(new DeleteMessageCommand(message.Id, Guid.NewGuid()), CancellationToken.None);

        Assert.False(result);
        await _messageRepository.DidNotReceive().Delete(Arg.Any<Guid>());
    }

    [Fact]
    public async Task Handle_WhenSenderIsAuthor_DeletesMessage()
    {
        var handler = CreateHandler();
        var message = new Message(AuthorId, ChannelId, "hello");
        _messageRepository.GetAsync(message.Id).Returns(message);

        var result = await handler.Handle(new DeleteMessageCommand(message.Id, AuthorId), CancellationToken.None);

        Assert.True(result);
        await _messageRepository.Received(1).Delete(message.Id);
    }
}
