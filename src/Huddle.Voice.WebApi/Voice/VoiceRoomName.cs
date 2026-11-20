namespace Huddle.Voice.WebApi.Voice;

/// <summary>
/// LiveKit room that belongs to a voice channel: <c>server-{serverId}-channel-{channelId}</c>.
/// </summary>
public readonly record struct VoiceRoomName(Guid ServerId, Guid ChannelId)
{
    private const string Prefix = "server-";
    private const string Separator = "-channel-";

    public static string Format(Guid serverId, Guid channelId) => $"{Prefix}{serverId}{Separator}{channelId}";

    public static bool TryParse(string? roomName, out VoiceRoomName room)
    {
        room = default;

        if (string.IsNullOrEmpty(roomName) || !roomName.StartsWith(Prefix, StringComparison.Ordinal))
            return false;

        var separator = roomName.IndexOf(Separator, Prefix.Length, StringComparison.Ordinal);
        if (separator < 0)
            return false;

        var serverIdStart = Prefix.Length;
        var channelIdStart = separator + Separator.Length;

        if (serverIdStart >= separator || channelIdStart >= roomName.Length)
            return false;

        if (!Guid.TryParse(roomName.AsSpan(serverIdStart, separator - serverIdStart), out var serverId)
            || !Guid.TryParse(roomName.AsSpan(channelIdStart), out var channelId))
            return false;

        room = new VoiceRoomName(serverId, channelId);
        return true;
    }
}
