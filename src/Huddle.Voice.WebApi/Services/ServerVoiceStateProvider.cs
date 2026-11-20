using Huddle.Voice.WebApi.Voice;

namespace Huddle.Voice.WebApi.Services;

/// <summary>
/// Serves the voice state of a server: the cached projection when it is warm, LiveKit otherwise.
/// </summary>
public sealed class ServerVoiceStateProvider(
    IVoiceStateStore stateStore,
    IVoiceRoomDirectory roomDirectory,
    ILogger<ServerVoiceStateProvider> logger)
{
    public async Task<ServerVoiceStateSnapshot> GetAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        try
        {
            var cached = await stateStore.GetAsync(serverId, cancellationToken);
            if (cached is not null)
                return cached;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            // The cache is only an optimisation: a cache outage must not hide the voice channels.
            logger.LogWarning(exception, "Voice state cache is unavailable, reading server {ServerId} from LiveKit", serverId);
        }

        try
        {
            var live = await roomDirectory.GetServerVoiceStateAsync(serverId, cancellationToken);

            return await TryReconcileAsync(serverId, live, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            // LiveKit is unreachable. Serving an empty state keeps the channel list usable; nothing is
            // cached, so the next request retries instead of showing an empty list for a full TTL.
            logger.LogWarning(exception, "Could not read the voice state of server {ServerId} from LiveKit", serverId);

            return ServerVoiceStateSnapshot.Empty;
        }
    }

    /// <summary>The data read from LiveKit is still useful when the cache cannot be updated, so it is served anyway.</summary>
    private async Task<ServerVoiceStateSnapshot> TryReconcileAsync(
        Guid serverId,
        IReadOnlyCollection<ChannelVoiceState> channels,
        CancellationToken cancellationToken)
    {
        try
        {
            return await stateStore.ReconcileAsync(serverId, channels, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogWarning(exception, "Could not cache the voice state of server {ServerId}", serverId);

            return new ServerVoiceStateSnapshot(0, channels.ToArray());
        }
    }
}
