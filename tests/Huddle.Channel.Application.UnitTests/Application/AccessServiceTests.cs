using Huddle.Channel.Application.Services;
using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using NSubstitute;

namespace Huddle.Channel.Application.UnitTests.Application;

public class AccessServiceTests
{
    private static readonly Guid IdentityId = Guid.NewGuid();

    private readonly IMemberRepository _memberRepository = Substitute.For<IMemberRepository>();
    private readonly IServerRepository _serverRepository = Substitute.For<IServerRepository>();

    private AccessService CreateService() => new(_memberRepository, _serverRepository);

    [Fact]
    public async Task CanUserAccessChannel_WithUnknownChannel_Throws()
    {
        var service = CreateService();
        _serverRepository.GetChannelAsync(Arg.Any<Guid>()).Returns((Huddle.Channel.Domain.Aggregates.ServerAggregate.Channel?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.CanUserAccessChannelAsync(Guid.NewGuid(), IdentityId));
    }

    [Fact]
    public async Task CanUserAccessChannel_WhenUserIsMember_ReturnsTrue()
    {
        var service = CreateService();
        var server = Builders.Server();
        var channel = server.Channels.First();
        _serverRepository.GetChannelAsync(channel.Id).Returns(channel);
        _memberRepository.GetByServerAndIdentityIdAsync(channel.ServerId, IdentityId)
            .Returns(Builders.Member(channel.ServerId));

        var hasAccess = await service.CanUserAccessChannelAsync(channel.Id, IdentityId);

        Assert.True(hasAccess);
    }

    [Fact]
    public async Task CanUserAccessChannel_WhenUserIsNotMember_ReturnsFalse()
    {
        var service = CreateService();
        var server = Builders.Server();
        var channel = server.Channels.First();
        _serverRepository.GetChannelAsync(channel.Id).Returns(channel);
        _memberRepository.GetByServerAndIdentityIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns((Member?)null);

        var hasAccess = await service.CanUserAccessChannelAsync(channel.Id, IdentityId);

        Assert.False(hasAccess);
    }

    [Fact]
    public async Task IsUserMemberOfServer_WhenMemberExists_ReturnsTrue()
    {
        var service = CreateService();
        var serverId = Guid.NewGuid();
        _memberRepository.GetByServerAndIdentityIdAsync(serverId, IdentityId).Returns(Builders.Member(serverId));

        Assert.True(await service.IsUserMemberOfServerAsync(serverId, IdentityId));
    }

    [Fact]
    public async Task IsUserMemberOfServer_WhenMemberDoesNotExist_ReturnsFalse()
    {
        var service = CreateService();
        _memberRepository.GetByServerAndIdentityIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns((Member?)null);

        Assert.False(await service.IsUserMemberOfServerAsync(Guid.NewGuid(), IdentityId));
    }
}
