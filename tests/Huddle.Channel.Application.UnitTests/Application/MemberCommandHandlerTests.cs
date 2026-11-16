using Huddle.Channel.Application.Commands.Member;
using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using Huddle.Channel.Domain.SeedWork;
using NSubstitute;

namespace Huddle.Channel.Application.UnitTests.Application;

/// <summary>Members get their id from the database, so the test sets it explicitly.</summary>
internal sealed class MemberWithId : Member
{
    public MemberWithId(Guid id, Guid serverId, Guid identityId) : base(serverId, identityId) => Id = id;
}

public class CreateMemberCommandHandlerTests
{
    private static readonly Guid ServerId = Guid.NewGuid();
    private static readonly Guid IdentityId = Guid.NewGuid();

    private readonly IMemberRepository _memberRepository = Substitute.For<IMemberRepository>();
    private readonly IServerRepository _serverRepository = Substitute.For<IServerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private CreateMemberCommandHandler CreateHandler()
    {
        _memberRepository.UnitOfWork.Returns(_unitOfWork);
        _memberRepository.Add(Arg.Any<Member>()).Returns(call => call.Arg<Member>());
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);

        return new CreateMemberCommandHandler(_memberRepository, _serverRepository);
    }

    [Fact]
    public async Task Handle_WithUnknownServer_Throws()
    {
        var handler = CreateHandler();
        _serverRepository.GetAsync(ServerId).Returns((Server?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.Handle(new CreateMemberCommand(ServerId, IdentityId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ForPrivateServer_Throws()
    {
        var handler = CreateHandler();
        _serverRepository.GetAsync(ServerId).Returns(new Server(Builders.OwnerId, "Huddle", isPrivate: true));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(new CreateMemberCommand(ServerId, IdentityId), CancellationToken.None));

        _memberRepository.DidNotReceive().Add(Arg.Any<Member>());
    }

    [Fact]
    public async Task Handle_AddsMemberAndSaves()
    {
        var handler = CreateHandler();
        _serverRepository.GetAsync(ServerId).Returns(Builders.Server());

        var result = await handler.Handle(new CreateMemberCommand(ServerId, IdentityId), CancellationToken.None);

        Assert.True(result);
        _memberRepository.Received(1).Add(Arg.Is<Member>(member =>
            member.ServerId == ServerId && member.IdentityId == IdentityId));
        await _unitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }
}

public class UpdateMemberCommandHandlerTests
{
    private readonly IMemberRepository _memberRepository = Substitute.For<IMemberRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private UpdateMemberCommandHandler CreateHandler()
    {
        _memberRepository.UnitOfWork.Returns(_unitOfWork);
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);
        return new UpdateMemberCommandHandler(_memberRepository);
    }

    [Fact]
    public async Task Handle_WithUnknownMember_Throws()
    {
        var handler = CreateHandler();
        _memberRepository.GetAsync(Arg.Any<Guid>()).Returns((Member?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(
            new UpdateMemberCommand(Guid.NewGuid(), "neo", Guid.NewGuid()),
            CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenSenderIsNotTheMember_ReturnsFalse()
    {
        var handler = CreateHandler();
        var member = new MemberWithId(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        _memberRepository.GetAsync(member.Id).Returns(member);

        var result = await handler.Handle(new UpdateMemberCommand(member.Id, "neo", Guid.NewGuid()), CancellationToken.None);

        Assert.False(result);
        Assert.Equal(string.Empty, member.Profile.SeverUsername);
    }

    [Fact]
    public async Task Handle_WhenSenderIsTheMember_UpdatesUsername()
    {
        var handler = CreateHandler();
        var member = new MemberWithId(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        _memberRepository.GetAsync(member.Id).Returns(member);

        var result = await handler.Handle(new UpdateMemberCommand(member.Id, "neo", member.Id), CancellationToken.None);

        Assert.True(result);
        Assert.Equal("neo", member.Profile.SeverUsername);
        await _unitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }
}
