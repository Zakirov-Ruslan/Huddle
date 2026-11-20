using Livekit.Server.Sdk.Dotnet;

namespace Huddle.Voice.WebApi.Voice;

/// <summary>
/// Queries the LiveKit RoomService for the participants of every voice channel of a server.
/// Used to rebuild the cache, so it is never on the hot path of the sidebar requests.
/// </summary>
public sealed class LiveKitVoiceRoomDirectory : IVoiceRoomDirectory
{
    private readonly RoomServiceClient _roomService;
    private readonly ILogger<LiveKitVoiceRoomDirectory> _logger;

    public LiveKitVoiceRoomDirectory(IConfiguration configuration, ILogger<LiveKitVoiceRoomDirectory> logger)
    {
        var liveKitUrl = configuration.GetValue<string>("LIVEKIT_URL")
            ?? throw new InvalidOperationException("LIVEKIT_URL environment variable not defined");
        var apiKey = configuration.GetValue<string>("LIVEKIT_API_KEY")
            ?? throw new InvalidOperationException("LIVEKIT_API_KEY environment variable not defined");
        var apiSecret = configuration.GetValue<string>("LIVEKIT_API_SECRET")
            ?? throw new InvalidOperationException("LIVEKIT_API_SECRET environment variable not defined");

        _roomService = new RoomServiceClient(liveKitUrl, apiKey, apiSecret);
        _logger = logger;
    }

    public async Task<IReadOnlyList<ChannelVoiceState>> GetServerVoiceStateAsync(
        Guid serverId,
        CancellationToken cancellationToken = default)
    {
        var roomsResponse = await _roomService.ListRooms(new ListRoomsRequest());

        var serverRooms = roomsResponse.Rooms
            .Select(room => (Room: room, Parsed: TryParseServerRoom(room.Name, serverId)))
            .Where(entry => entry.Parsed is not null)
            .ToArray();

        if (serverRooms.Length == 0)
            return [];

        var channels = await Task.WhenAll(serverRooms.Select(entry => ReadChannelAsync(entry.Room, entry.Parsed!.Value)));

        return channels
            .Where(channel => channel is { Participants.Count: > 0 })
            .Select(channel => channel!)
            .ToArray();
    }

    /// <summary>
    /// A room can disappear between listing the rooms and listing its participants. Skipping such a room
    /// keeps the rest of the server readable instead of failing the whole snapshot.
    /// </summary>
    private async Task<ChannelVoiceState?> ReadChannelAsync(Room room, VoiceRoomName parsedRoom)
    {
        try
        {
            var participants = await _roomService.ListParticipants(new ListParticipantsRequest { Room = room.Name });

            return new ChannelVoiceState(
                parsedRoom.ChannelId,
                participants.Participants.Select(MapParticipant).ToArray());
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogWarning(exception, "Could not read participants of the LiveKit room {RoomName}", room.Name);

            return null;
        }
    }

    private static VoiceRoomName? TryParseServerRoom(string roomName, Guid serverId) =>
        VoiceRoomName.TryParse(roomName, out var room) && room.ServerId == serverId ? room : null;

    private static VoiceParticipant MapParticipant(ParticipantInfo participant) =>
        new(
            Guid.TryParse(participant.Identity, out var userId) ? userId : Guid.Empty,
            string.IsNullOrWhiteSpace(participant.Name) ? participant.Identity : participant.Name,
            ToTimestamp(participant.JoinedAt));

    private static DateTimeOffset ToTimestamp(long unixSeconds) =>
        unixSeconds > 0 ? DateTimeOffset.FromUnixTimeSeconds(unixSeconds) : DateTimeOffset.UtcNow;
}
