namespace Huddle.Voice.WebApi.Voice;

/// <summary>Reads the live state of the voice channels from LiveKit, the source of truth.</summary>
public interface IVoiceRoomDirectory
{
    Task<IReadOnlyList<ChannelVoiceState>> GetServerVoiceStateAsync(
        Guid serverId,
        CancellationToken cancellationToken = default);
}
