using Huddle.Channel.Application.Commands.Message;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Exceptions;
using Huddle.Channel.Application.Mapping;
using Huddle.Channel.Application.Services;
using Huddle.Channel.Domain.Aggregates.MessageAggregate;
using Huddle.Channel.Domain.SeedWork;
using NSubstitute;

namespace Huddle.Channel.Application.UnitTests.Application;

public class CreateMessageCommandHandlerTests
{
    private static readonly Guid AuthorId = Guid.NewGuid();
    private static readonly Guid ChannelId = Guid.NewGuid();

    private readonly IMessageRepository _messageRepository = Substitute.For<IMessageRepository>();
    private readonly IMessageDtoFactory _dtoFactory = Substitute.For<IMessageDtoFactory>();
    private readonly IAccessService _accessService = Substitute.For<IAccessService>();
    private readonly IFilesClient _filesClient = Substitute.For<IFilesClient>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly MessageDto _expectedDto = new(Guid.NewGuid(), ChannelId, AuthorId, "hello", DateTime.UtcNow, false, []);

    private Message? _addedMessage;

    private CreateMessageCommandHandler CreateHandler()
    {
        _messageRepository.UnitOfWork.Returns(_unitOfWork);
        _messageRepository.Add(Arg.Any<Message>()).Returns(call =>
        {
            _addedMessage = call.Arg<Message>();
            return _addedMessage;
        });
        _dtoFactory.Create(Arg.Any<Message>()).Returns(_expectedDto);
        _accessService.CanUserAccessChannelAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);

        return new CreateMessageCommandHandler(_messageRepository, _dtoFactory, _accessService, _filesClient);
    }

    [Fact]
    public async Task Handle_WithoutChannelAccess_Throws()
    {
        var handler = CreateHandler();
        _accessService.CanUserAccessChannelAsync(ChannelId, AuthorId).Returns(false);

        await Assert.ThrowsAsync<ForbiddenAccessException>(
            () => handler.Handle(new CreateMessageCommand(AuthorId, ChannelId, "hello"), CancellationToken.None));

        _messageRepository.DidNotReceive().Add(Arg.Any<Message>());
        await _unitOfWork.DidNotReceive().SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithoutAttachments_SavesMessageAndReturnsDto()
    {
        var handler = CreateHandler();

        var result = await handler.Handle(new CreateMessageCommand(AuthorId, ChannelId, "hello"), CancellationToken.None);

        Assert.Same(_expectedDto, result);
        Assert.NotNull(_addedMessage);
        Assert.Equal(AuthorId, _addedMessage.AuthorId);
        Assert.Equal(ChannelId, _addedMessage.ChannelId);
        Assert.Equal("hello", _addedMessage.Text);
        Assert.Empty(_addedMessage.Attachments);
        await _filesClient.DidNotReceive()
            .ClaimAttachmentsAsync(Arg.Any<Guid>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithAttachments_ClaimsFilesAndMapsThemInOrder()
    {
        var handler = CreateHandler();
        var attachmentIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var claimed = attachmentIds
            .Select((id, index) => new ClaimedFile(id, $"file-{index}.png", "image/png", 100 + index, IsImage: true))
            .ToList();
        _filesClient
            .ClaimAttachmentsAsync(AuthorId, Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(claimed);

        await handler.Handle(new CreateMessageCommand(AuthorId, ChannelId, "hello", attachmentIds), CancellationToken.None);

        await _filesClient.Received(1).ClaimAttachmentsAsync(
            AuthorId,
            Arg.Is<IReadOnlyList<Guid>>(ids => ids.SequenceEqual(attachmentIds)),
            Arg.Any<CancellationToken>());

        Assert.NotNull(_addedMessage);
        var attachments = _addedMessage.Attachments.ToList();
        Assert.Equal(2, attachments.Count);
        Assert.Equal(claimed[0].Id, attachments[0].FileId);
        Assert.Equal("file-0.png", attachments[0].FileName);
        Assert.Equal(0, attachments[0].Position);
        Assert.Equal(claimed[1].Id, attachments[1].FileId);
        Assert.Equal(1, attachments[1].Position);
    }
}
