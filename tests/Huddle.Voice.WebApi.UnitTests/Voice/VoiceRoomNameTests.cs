using Huddle.Voice.WebApi.Voice;

namespace Huddle.Voice.WebApi.UnitTests.Voice;

public class VoiceRoomNameTests
{
    [Fact]
    public void Format_RoundTripsThroughParse()
    {
        var serverId = Guid.NewGuid();
        var channelId = Guid.NewGuid();

        var roomName = VoiceRoomName.Format(serverId, channelId);

        Assert.True(VoiceRoomName.TryParse(roomName, out var parsed));
        Assert.Equal(serverId, parsed.ServerId);
        Assert.Equal(channelId, parsed.ChannelId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("some-room")]
    [InlineData("server-not-a-guid-channel-not-a-guid")]
    [InlineData("server-8f1c4e5d-6b6a-4a3c-9a5a-6f0a1d2e3f40-channel-")]
    public void TryParse_RejectsNamesThatAreNotServerRooms(string? roomName)
    {
        Assert.False(VoiceRoomName.TryParse(roomName, out _));
    }
}
