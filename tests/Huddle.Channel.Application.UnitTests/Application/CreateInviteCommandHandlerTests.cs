using AutoMapper;
using Huddle.Channel.Application.Commands.Invite;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.Exceptions;
using Huddle.Channel.Application.Services;
using Huddle.Channel.Domain.Aggregates.InviteAggregate;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using Huddle.Channel.Domain.SeedWork;
using NSubstitute;

namespace Huddle.Channel.Application.UnitTests.Application;

public class CreateInviteCommandHandlerTests
{
    private static readonly Guid ServerId = Guid.NewGuid();
    private static readonly InviteDto ExpectedDto = new(Guid.NewGuid(), ServerId, DateTime.UtcNow, "abc1234567");

    private readonly IInviteRepository _inviteRepository = Substitute.For<IInviteRepository>();
    private readonly IServerRepository _serverRepository = Substitute.For<IServerRepository>();
    private readonly IShortIdService _shortIdService = Substitute.For<IShortIdService>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private Invite? _addedInvite;

    private CreateInviteCommandHandler CreateHandler()
    {
        _inviteRepository.UnitOfWork.Returns(_unitOfWork);
        _inviteRepository.Add(Arg.Any<Invite>()).Returns(call =>
        {
            _addedInvite = call.Arg<Invite>();
            return _addedInvite;
        });
        _inviteRepository.GetBySeverId(ServerId).Returns([]);
        _mapper.Map<InviteDto>(Arg.Any<Invite>()).Returns(ExpectedDto);

        return new CreateInviteCommandHandler(_inviteRepository, _shortIdService, _serverRepository, _mapper);
    }

    [Fact]
    public async Task Handle_WithUnknownServer_Throws()
    {
        var handler = CreateHandler();
        _serverRepository.GetAsync(ServerId).Returns((Server?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.Handle(new CreateInviteCommand(ServerId, Builders.OwnerId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenSenderIsNotServerOwner_Throws()
    {
        var handler = CreateHandler();
        _serverRepository.GetAsync(ServerId).Returns(Builders.Server());

        await Assert.ThrowsAsync<ForbiddenAccessException>(
            () => handler.Handle(new CreateInviteCommand(ServerId, Guid.NewGuid()), CancellationToken.None));

        _inviteRepository.DidNotReceive().Add(Arg.Any<Invite>());
    }

    [Fact]
    public async Task Handle_WhenServerAlreadyHasInvite_ReturnsItWithoutCreatingAnother()
    {
        var handler = CreateHandler();
        _serverRepository.GetAsync(ServerId).Returns(Builders.Server());
        _inviteRepository.GetBySeverId(ServerId).Returns([new Invite(ServerId, "existing123")]);

        var result = await handler.Handle(new CreateInviteCommand(ServerId, Builders.OwnerId), CancellationToken.None);

        Assert.Same(ExpectedDto, result);
        _shortIdService.DidNotReceive().GetShortId();
        _inviteRepository.DidNotReceive().Add(Arg.Any<Invite>());
    }

    [Fact]
    public async Task Handle_WithFreeCode_CreatesInviteWithGeneratedCode()
    {
        var handler = CreateHandler();
        _serverRepository.GetAsync(ServerId).Returns(Builders.Server());
        _shortIdService.GetShortId().Returns("abc1234567");
        _inviteRepository.GetByCode("abc1234567").Returns((Invite?)null);

        var result = await handler.Handle(new CreateInviteCommand(ServerId, Builders.OwnerId), CancellationToken.None);

        Assert.Same(ExpectedDto, result);
        Assert.NotNull(_addedInvite);
        Assert.Equal(ServerId, _addedInvite.ServerId);
        Assert.Equal("abc1234567", _addedInvite.Code);
        await _unitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenFirstCodeCollides_RetriesUntilFreeCode()
    {
        var handler = CreateHandler();
        _serverRepository.GetAsync(ServerId).Returns(Builders.Server());
        _shortIdService.GetShortId().Returns("taken11111", "taken22222", "free333333");
        _inviteRepository.GetByCode("taken11111").Returns(new Invite(ServerId, "taken11111"));
        _inviteRepository.GetByCode("taken22222").Returns(new Invite(ServerId, "taken22222"));
        _inviteRepository.GetByCode("free333333").Returns((Invite?)null);

        await handler.Handle(new CreateInviteCommand(ServerId, Builders.OwnerId), CancellationToken.None);

        Assert.NotNull(_addedInvite);
        Assert.Equal("free333333", _addedInvite.Code);
        _shortIdService.Received(3).GetShortId();
    }

    [Fact]
    public async Task Handle_WhenAllCodesCollide_Throws()
    {
        var handler = CreateHandler();
        _serverRepository.GetAsync(ServerId).Returns(Builders.Server());
        _shortIdService.GetShortId().Returns("taken11111");
        _inviteRepository.GetByCode("taken11111").Returns(new Invite(ServerId, "taken11111"));

        await Assert.ThrowsAsync<Exception>(
            () => handler.Handle(new CreateInviteCommand(ServerId, Builders.OwnerId), CancellationToken.None));

        _shortIdService.Received(5).GetShortId();
        _inviteRepository.DidNotReceive().Add(Arg.Any<Invite>());
    }
}
