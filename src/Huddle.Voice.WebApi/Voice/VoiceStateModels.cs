namespace Huddle.Voice.WebApi.Voice;

/// <summary>Participant of a voice channel as reported by LiveKit.</summary>
public sealed record VoiceParticipant(Guid UserId, string DisplayName, DateTimeOffset JoinedAt);

/// <summary>Participants of a single voice channel.</summary>
public sealed record ChannelVoiceState(Guid ChannelId, IReadOnlyList<VoiceParticipant> Participants);

/// <summary>
/// Point-in-time state of all voice channels of a server, together with the version it was produced in.
/// </summary>
/// <remarks>
/// The version increases with every change of the state, so a client that already applied version N can
/// drop any update that arrives with version N or lower - typically a push that raced with a fresh snapshot.
/// </remarks>
public sealed record ServerVoiceStateSnapshot(long Version, IReadOnlyList<ChannelVoiceState> Channels)
{
    public static ServerVoiceStateSnapshot Empty { get; } = new(0, []);
}
