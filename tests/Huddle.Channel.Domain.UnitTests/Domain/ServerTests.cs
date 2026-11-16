using Huddle.Channel.Domain.Aggregates.Events;
using Huddle.Channel.Domain.Aggregates.Events.Server;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;

namespace Huddle.Channel.Domain.UnitTests.Domain;

public class ServerTests
{
    private static readonly Guid OwnerId = Guid.NewGuid();

    [Fact]
    public void Constructor_SetsProperties()
    {
        var server = new Server(OwnerId, "Huddle", isPrivate: true);

        Assert.NotEqual(Guid.Empty, server.Id);
        Assert.Equal("Huddle", server.Name);
        Assert.Equal(OwnerId, server.OwnerIdentityId);
        Assert.True(server.IsPrivate);
        Assert.Null(server.AvatarFileId);
    }

    [Fact]
    public void Constructor_CreatesMainTextAndVoiceChannels()
    {
        var server = new Server(OwnerId, "Huddle", isPrivate: false);

        Assert.Equal(2, server.Channels.Count());
        Assert.Contains(server.Channels, channel => channel.Type == ChannelType.Text && channel.Name == "Main");
        Assert.Contains(server.Channels, channel => channel.Type == ChannelType.Voice && channel.Name == "Main");
    }

    [Fact]
    public void Constructor_RaisesServerCreatedDomainEvent()
    {
        var server = new Server(OwnerId, "Huddle", isPrivate: false);

        Assert.NotNull(server.DomainEvents);
        var domainEvent = Assert.Single(server.DomainEvents.OfType<ServerCreatedDomainEvent>());
        Assert.Equal(server.Id, domainEvent.ServerId);
        Assert.Equal("Huddle", domainEvent.Name);
        Assert.Equal(OwnerId, domainEvent.OwnerIdentityId);
        Assert.Equal(2, server.DomainEvents.OfType<ChannelCreatedDomainEvent>().Count());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_Throws(string? name)
    {
        Assert.Throws<ArgumentException>(() => new Server(OwnerId, name!, isPrivate: false));
    }

    [Fact]
    public void Constructor_WithTooLongName_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Server(OwnerId, new string('a', 31), isPrivate: false));
    }

    [Fact]
    public void Constructor_WithMaxLengthName_Succeeds()
    {
        var server = new Server(OwnerId, new string('a', 30), isPrivate: false);

        Assert.Equal(30, server.Name.Length);
    }

    [Fact]
    public void UpdateName_ChangesNameAndRaisesDomainEvent()
    {
        var server = new Server(OwnerId, "Huddle", isPrivate: false);
        server.ClearDomainEvents();

        server.UpdateName("Renamed");

        Assert.Equal("Renamed", server.Name);
        Assert.NotNull(server.DomainEvents);
        var domainEvent = Assert.IsType<ServerUpdatedDomainEvent>(Assert.Single(server.DomainEvents));
        Assert.Equal(server.Id, domainEvent.Id);
        Assert.Equal("Renamed", domainEvent.Name);
    }

    [Fact]
    public void UpdateName_WithInvalidName_ThrowsAndKeepsName()
    {
        var server = new Server(OwnerId, "Huddle", isPrivate: false);

        Assert.Throws<ArgumentException>(() => server.UpdateName(""));
        Assert.Equal("Huddle", server.Name);
    }

    [Fact]
    public void AddChannel_AddsChannelAndRaisesDomainEvent()
    {
        var server = new Server(OwnerId, "Huddle", isPrivate: false);
        server.ClearDomainEvents();

        var channel = server.AddChannel(ChannelType.Text, "general");

        Assert.Contains(channel, server.Channels);
        Assert.Equal("general", channel.Name);
        Assert.Equal(ChannelType.Text, channel.Type);
        Assert.NotNull(server.DomainEvents);
        var domainEvent = Assert.IsType<ChannelCreatedDomainEvent>(Assert.Single(server.DomainEvents));
        Assert.Equal("general", domainEvent.Name);
        Assert.Equal(ChannelType.Text, domainEvent.Type);
        Assert.Equal(server.Id, domainEvent.ServerId);
    }

    [Fact]
    public void AddChannel_WithInvalidName_Throws()
    {
        var server = new Server(OwnerId, "Huddle", isPrivate: false);

        Assert.Throws<ArgumentException>(() => server.AddChannel(ChannelType.Text, ""));
    }

    [Fact]
    public void DeleteChannel_RemovesChannelAndRaisesDomainEvent()
    {
        var server = new Server(OwnerId, "Huddle", isPrivate: false);
        var channel = server.Channels.First();
        server.ClearDomainEvents();

        server.DeleteChannel(channel.Id);

        Assert.DoesNotContain(channel, server.Channels);
        Assert.NotNull(server.DomainEvents);
        var domainEvent = Assert.IsType<ChannelDeletedDomainEvent>(Assert.Single(server.DomainEvents));
        Assert.Equal(channel.Id, domainEvent.ChannelId);
        Assert.Equal(server.Id, domainEvent.ServerId);
    }

    [Fact]
    public void DeleteChannel_WithUnknownId_Throws()
    {
        var server = new Server(OwnerId, "Huddle", isPrivate: false);

        Assert.Throws<InvalidOperationException>(() => server.DeleteChannel(Guid.NewGuid()));
    }

    [Fact]
    public void SetAvatar_Then_RemoveAvatar_UpdatesAvatarFileId()
    {
        var server = new Server(OwnerId, "Huddle", isPrivate: false);
        var fileId = Guid.NewGuid();

        server.SetAvatar(fileId);
        Assert.Equal(fileId, server.AvatarFileId);

        server.RemoveAvatar();
        Assert.Null(server.AvatarFileId);
    }
}
