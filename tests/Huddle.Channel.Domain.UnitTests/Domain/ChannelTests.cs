using Huddle.Channel.Domain.Aggregates.Events;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;

namespace Huddle.Channel.Domain.UnitTests.Domain;

public class ChannelTests
{
    [Fact]
    public void ChangeName_UpdatesNameAndRaisesDomainEvent()
    {
        var server = new Server(Guid.NewGuid(), "Huddle", isPrivate: false);
        var channel = server.AddChannel(ChannelType.Text, "general");
        channel.ClearDomainEvents();

        channel.ChangeName("random");

        Assert.Equal("random", channel.Name);
        Assert.NotNull(channel.DomainEvents);
        var domainEvent = Assert.IsType<ChannelUpdatedDomainEvent>(Assert.Single(channel.DomainEvents));
        Assert.Equal("random", domainEvent.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ChangeName_WithInvalidName_Throws(string? name)
    {
        var server = new Server(Guid.NewGuid(), "Huddle", isPrivate: false);
        var channel = server.AddChannel(ChannelType.Text, "general");

        Assert.Throws<ArgumentException>(() => channel.ChangeName(name!));
        Assert.Equal("general", channel.Name);
    }

    [Fact]
    public void ChangeName_WithTooLongName_Throws()
    {
        var server = new Server(Guid.NewGuid(), "Huddle", isPrivate: false);
        var channel = server.AddChannel(ChannelType.Voice, "General");

        Assert.Throws<ArgumentException>(() => channel.ChangeName(new string('a', 31)));
    }
}
