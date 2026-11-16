using Huddle.Channel.Domain.Aggregates.InviteAggregate;
using Huddle.Channel.Domain.Aggregates.MemberAggregate;

namespace Huddle.Channel.Domain.UnitTests.Domain;

public class InviteTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var serverId = Guid.NewGuid();
        var before = DateTime.UtcNow;

        var invite = new Invite(serverId, "abcd123456");

        Assert.Equal(serverId, invite.ServerId);
        Assert.Equal("abcd123456", invite.Code);
        Assert.InRange(invite.CreatedAt, before, DateTime.UtcNow);
    }
}

public class MemberTests
{
    [Fact]
    public void Constructor_SetsPropertiesAndEmptyProfile()
    {
        var serverId = Guid.NewGuid();
        var identityId = Guid.NewGuid();
        var before = DateTime.UtcNow;

        var member = new Member(serverId, identityId);

        Assert.Equal(serverId, member.ServerId);
        Assert.Equal(identityId, member.IdentityId);
        Assert.InRange(member.CreatedAt, before, DateTime.UtcNow);
        Assert.NotNull(member.Profile);
        Assert.Equal(string.Empty, member.Profile.SeverUsername);
        Assert.Null(member.DomainEvents);
    }
}

public class MemberProfileTests
{
    [Fact]
    public void ChangeServerUsername_SetsUsername()
    {
        var profile = new MemberProfile();

        profile.ChangeServerUsername("neo");

        Assert.Equal("neo", profile.SeverUsername);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ChangeServerUsername_WithInvalidValue_Throws(string? username)
    {
        var profile = new MemberProfile();

        Assert.Throws<ArgumentException>(() => profile.ChangeServerUsername(username!));
    }

    [Fact]
    public void ChangeServerUsername_WithTooLongValue_Throws()
    {
        var profile = new MemberProfile();

        Assert.Throws<ArgumentException>(() => profile.ChangeServerUsername(new string('a', 21)));
    }

    [Fact]
    public void ChangeDescription_SetsDescription()
    {
        var profile = new MemberProfile();

        profile.ChangeDescription("backend dev");

        Assert.Equal("backend dev", profile.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ChangeDescription_WithInvalidValue_Throws(string? description)
    {
        var profile = new MemberProfile();

        Assert.Throws<ArgumentException>(() => profile.ChangeDescription(description!));
    }
}
