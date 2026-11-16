using Huddle.Channel.Application.Commands.Invite;
using Huddle.Channel.Domain.Aggregates.InviteAggregate;
using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using Huddle.Channel.Domain.SeedWork;
using NSubstitute;

namespace Huddle.Channel.Application.UnitTests.Application;

public class AcceptInviteCommandHandlerTests
{
    private static readonly Guid ServerId = Guid.NewGuid();
    private static readonly Guid IdentityId = Guid.NewGuid();
    private const string InviteCode = "abcd123456";

    private readonly IInviteRepository _inviteRepository = Substitute.For<IInviteRepository>();
    private readonly IMemberRepository _memberRepository = Substitute.For<IMemberRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private AcceptInviteCommandHandler CreateHandler()
    {
        _memberRepository.UnitOfWork.Returns(_unitOfWork);
        _memberRepository.Add(Arg.Any<Member>()).Returns(call => call.Arg<Member>());
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);

        return new AcceptInviteCommandHandler(_inviteRepository, _memberRepository);
    }

    [Fact]
    public async Task Handle_WithUnknownCode_Throws()
    {
        var handler = CreateHandler();
        _inviteRepository.GetByCode(InviteCode).Returns((Invite?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.Handle(new AcceptInviteCommand(InviteCode, IdentityId), CancellationToken.None));

        _memberRepository.DidNotReceive().Add(Arg.Any<Member>());
    }

    [Fact]
    public async Task Handle_WhenUserIsAlreadyMember_ReturnsServerIdWithoutAddingMember()
    {
        var handler = CreateHandler();
        _inviteRepository.GetByCode(InviteCode).Returns(new Invite(ServerId, InviteCode));
        _memberRepository.GetByServerAndIdentityIdAsync(ServerId, IdentityId).Returns(Builders.Member(ServerId, IdentityId));

        var result = await handler.Handle(new AcceptInviteCommand(InviteCode, IdentityId), CancellationToken.None);

        Assert.Equal(ServerId, result);
        _memberRepository.DidNotReceive().Add(Arg.Any<Member>());
    }

    [Fact]
    public async Task Handle_AddsMemberAndReturnsServerId()
    {
        var handler = CreateHandler();
        _inviteRepository.GetByCode(InviteCode).Returns(new Invite(ServerId, InviteCode));
        _memberRepository.GetByServerAndIdentityIdAsync(ServerId, IdentityId).Returns((Member?)null);

        var result = await handler.Handle(new AcceptInviteCommand(InviteCode, IdentityId), CancellationToken.None);

        Assert.Equal(ServerId, result);
        _memberRepository.Received(1).Add(Arg.Is<Member>(member =>
            member.ServerId == ServerId && member.IdentityId == IdentityId));
        await _unitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }
}
