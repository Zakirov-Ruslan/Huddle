using Huddle.Channel.Application.Commands;
using Huddle.Channel.Application.Commands.Invite;
using Huddle.Channel.Application.Exceptions;
using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Huddle.Channel.Application.UnitTests.Application;

public class IdentifiedCommandHandlerTests
{
    private static readonly Guid RequestId = Guid.NewGuid();
    private static readonly Guid ServerId = Guid.NewGuid();

    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly IRequestManager _requestManager = Substitute.For<IRequestManager>();
    private readonly IMemberRepository _memberRepository = Substitute.For<IMemberRepository>();

    private AcceptInviteIdentifiedCommandHandler CreateHandler() =>
        new(_mediator, _requestManager, NullLogger<AcceptInviteIdentifiedCommandHandler>.Instance, _memberRepository);

    private static IdentifiedCommand<AcceptInviteCommand, Guid> CreateCommand() =>
        new(new AcceptInviteCommand("abcd123456", Guid.NewGuid()), RequestId);

    [Fact]
    public async Task Handle_WhenRequestWasAlreadyProcessed_ReturnsDuplicateResultWithoutSendingCommand()
    {
        var handler = CreateHandler();
        _requestManager.IsExistsAsync(RequestId).Returns(true);

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        Assert.Equal(Guid.Empty, result);
        await _mediator.DidNotReceive().Send(Arg.Any<IRequest<Guid>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRequestIsNew_RegistersRequestAndSendsCommand()
    {
        var handler = CreateHandler();
        var command = CreateCommand();
        _requestManager.IsExistsAsync(RequestId).Returns(false);
        _mediator.Send(Arg.Any<IRequest<Guid>>(), Arg.Any<CancellationToken>()).Returns(ServerId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(ServerId, result);
        await _requestManager.Received(1).CreateRequestForCommandAsync<AcceptInviteCommand>(RequestId);
        await _mediator.Received(1).Send(command.Command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenParallelRequestIsBeingProcessed_ReturnsDefault()
    {
        var handler = CreateHandler();
        _requestManager.IsExistsAsync(RequestId).Returns(false);
        _requestManager.CreateRequestForCommandAsync<AcceptInviteCommand>(Arg.Any<Guid>())
            .Returns(Task.FromException(new ParallelIdempotentRequestException()));

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        Assert.Equal(Guid.Empty, result);
        await _mediator.DidNotReceive().Send(Arg.Any<IRequest<Guid>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenInnerCommandFails_ReturnsDefault()
    {
        var handler = CreateHandler();
        _requestManager.IsExistsAsync(RequestId).Returns(false);
        _mediator.Send(Arg.Any<IRequest<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Guid>(new KeyNotFoundException()));

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        Assert.Equal(Guid.Empty, result);
    }
}
