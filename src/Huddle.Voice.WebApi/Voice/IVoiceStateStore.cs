namespace Huddle.Voice.WebApi.Voice;

/// <summary>
/// Cache of the voice state of a server. LiveKit stays the source of truth, the store only
/// keeps a projection that is rebuilt from it whenever the projection is missing.
/// </summary>
public interface IVoiceStateStore
{
    /// <summary>Reads the projection. <c>null</c> means the store has no entry for the server yet.</summary>
    Task<ServerVoiceStateSnapshot?> GetAsync(Guid serverId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reconciles the projection with the state just read from LiveKit and returns the result.
    /// </summary>
    Task<ServerVoiceStateSnapshot> ReconcileAsync(
        Guid serverId,
        IReadOnlyCollection<ChannelVoiceState> channels,
        CancellationToken cancellationToken = default);

    /// <summary>Records a join and returns the version of the resulting state.</summary>
    Task<long> AddParticipantAsync(
        Guid serverId,
        Guid channelId,
        VoiceParticipant participant,
        CancellationToken cancellationToken = default);

    /// <summary>Records a leave and returns the version of the resulting state.</summary>
    Task<long> RemoveParticipantAsync(
        Guid serverId,
        Guid channelId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>Drops every participant of a channel (the room is gone) and returns the version of the resulting state.</summary>
    Task<long> RemoveChannelAsync(Guid serverId, Guid channelId, CancellationToken cancellationToken = default);
}
