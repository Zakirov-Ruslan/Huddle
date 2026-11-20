using StackExchange.Redis;
using System.Text.Json;

namespace Huddle.Voice.WebApi.Voice;

/// <summary>
/// Redis projection of the LiveKit state.
/// </summary>
/// <remarks>
/// <para>
/// One hash per server: <c>voice:server:{serverId}</c>. Participants are fields named
/// <c>{channelId}:{userId}</c>, the <c>meta</c> field holds the version of the whole snapshot.
/// Every mutation increments the version inside the same transaction, so the pushes sent to clients
/// can be ordered against the snapshot they received before.
/// </para>
/// <para>
/// The hash expires on its own. Losing it is not an error: the next read rebuilds the projection from
/// LiveKit, which remains the source of truth. The sliding expiry also means that a missed webhook
/// stops being visible at most one <see cref="StateTtl"/> later.
/// </para>
/// </remarks>
public sealed class RedisVoiceStateStore : IVoiceStateStore
{
    private static readonly TimeSpan StateTtl = TimeSpan.FromMinutes(2);
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private const string MetaField = "meta";

    private readonly IDatabase _database;
    private readonly ILogger<RedisVoiceStateStore> _logger;

    public RedisVoiceStateStore(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisVoiceStateStore> logger)
    {
        _database = connectionMultiplexer.GetDatabase();
        _logger = logger;
    }

    public async Task<ServerVoiceStateSnapshot?> GetAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        var key = StateKey(serverId);
        var entries = await _database.HashGetAllAsync(key);

        // A missing key means "this server was never projected", not "nobody is in voice":
        // the projection always keeps the meta field once it exists.
        if (entries.Length == 0)
            return null;

        var version = 0L;
        var participants = new List<(Guid ChannelId, VoiceParticipant Participant)>(entries.Length);

        foreach (var entry in entries)
        {
            var field = entry.Name.ToString();

            if (field == MetaField)
            {
                long.TryParse(entry.Value.ToString(), out version);
                continue;
            }

            if (!TryParseParticipantField(field, out var channelId, out var userId))
            {
                _logger.LogWarning("Skipping unexpected field {Field} of the voice state of server {ServerId}", field, serverId);
                continue;
            }

            var participant = Deserialize(entry.Value, userId);
            if (participant is not null)
                participants.Add((channelId, participant));
        }

        await _database.KeyExpireAsync(key, StateTtl);

        return BuildSnapshot(version, participants);
    }

    public async Task<ServerVoiceStateSnapshot> ReconcileAsync(
        Guid serverId,
        IReadOnlyCollection<ChannelVoiceState> channels,
        CancellationToken cancellationToken = default)
    {
        var key = StateKey(serverId);
        var current = await ReadParticipantsAsync(key);

        var desired = new Dictionary<RedisValue, VoiceParticipant>();
        foreach (var channel in channels)
        {
            foreach (var participant in channel.Participants)
                desired[ParticipantField(channel.ChannelId, participant.UserId)] = participant;
        }

        var stale = current.Participants.Keys.Where(field => !desired.ContainsKey(field)).ToArray();
        var changed = desired
            .Where(pair => !current.Participants.TryGetValue(pair.Key, out var existing) || existing != pair.Value)
            .ToArray();

        if (stale.Length == 0 && changed.Length == 0)
        {
            await _database.KeyExpireAsync(key, StateTtl);
            return new ServerVoiceStateSnapshot(current.Version, Normalize(channels));
        }

        var version = await MutateAsync(serverId, (transaction, stateKey) =>
        {
            if (stale.Length > 0)
                _ = transaction.HashDeleteAsync(stateKey, stale);

            if (changed.Length > 0)
                _ = transaction.HashSetAsync(stateKey, changed.Select(pair => new HashEntry(pair.Key, Serialize(pair.Value))).ToArray());
        });

        return new ServerVoiceStateSnapshot(version, Normalize(channels));
    }

    public Task<long> AddParticipantAsync(
        Guid serverId,
        Guid channelId,
        VoiceParticipant participant,
        CancellationToken cancellationToken = default) =>
        MutateAsync(serverId, (transaction, key) =>
            _ = transaction.HashSetAsync(key, ParticipantField(channelId, participant.UserId), Serialize(participant)));

    public Task<long> RemoveParticipantAsync(
        Guid serverId,
        Guid channelId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        MutateAsync(serverId, (transaction, key) =>
            _ = transaction.HashDeleteAsync(key, ParticipantField(channelId, userId)));

    public async Task<long> RemoveChannelAsync(Guid serverId, Guid channelId, CancellationToken cancellationToken = default)
    {
        var key = StateKey(serverId);
        var entries = await _database.HashKeysAsync(key);

        var fields = entries
            .Select(field => field.ToString())
            .Where(field => TryParseParticipantField(field, out var parsedChannelId, out _) && parsedChannelId == channelId)
            .Select(field => (RedisValue)field)
            .ToArray();

        if (fields.Length == 0)
        {
            await _database.KeyExpireAsync(key, StateTtl);

            var currentVersion = await _database.HashGetAsync(key, MetaField);
            return long.TryParse(currentVersion.ToString(), out var parsedVersion) ? parsedVersion : 0;
        }

        return await MutateAsync(serverId, (transaction, stateKey) =>
            _ = transaction.HashDeleteAsync(stateKey, fields));
    }

    /// <summary>Applies one change and bumps the version in the same transaction.</summary>
    private async Task<long> MutateAsync(Guid serverId, Action<ITransaction, RedisKey> mutate)
    {
        var key = StateKey(serverId);
        var transaction = _database.CreateTransaction();

        mutate(transaction, key);

        var version = transaction.HashIncrementAsync(key, MetaField, 1);
        _ = transaction.KeyExpireAsync(key, StateTtl);

        if (!await transaction.ExecuteAsync())
            throw new InvalidOperationException($"Voice state of server {serverId} could not be updated");

        return await version;
    }

    private async Task<(long Version, Dictionary<RedisValue, VoiceParticipant> Participants)> ReadParticipantsAsync(RedisKey key)
    {
        var entries = await _database.HashGetAllAsync(key);
        var version = 0L;
        var participants = new Dictionary<RedisValue, VoiceParticipant>(entries.Length);

        foreach (var entry in entries)
        {
            if (entry.Name == MetaField)
            {
                long.TryParse(entry.Value.ToString(), out version);
                continue;
            }

            var participant = TryParseParticipantField(entry.Name.ToString(), out _, out var userId)
                ? Deserialize(entry.Value, userId)
                : null;

            if (participant is not null)
                participants[entry.Name] = participant;
        }

        return (version, participants);
    }

    private static ServerVoiceStateSnapshot BuildSnapshot(
        long version,
        IEnumerable<(Guid ChannelId, VoiceParticipant Participant)> participants) =>
        new(
            version,
            participants
                .GroupBy(entry => entry.ChannelId)
                .Select(group => new ChannelVoiceState(group.Key, group.Select(entry => entry.Participant).ToArray()))
                .ToArray());

    /// <summary>Empty channels are not part of the projection: the client keeps its own channel list.</summary>
    private static IReadOnlyList<ChannelVoiceState> Normalize(IEnumerable<ChannelVoiceState> channels) =>
        channels.Where(channel => channel.Participants.Count > 0).ToArray();

    private static RedisKey StateKey(Guid serverId) => $"voice:server:{serverId}";

    private static RedisValue ParticipantField(Guid channelId, Guid userId) => $"{channelId}:{userId}";

    private static bool TryParseParticipantField(string field, out Guid channelId, out Guid userId)
    {
        channelId = Guid.Empty;
        userId = Guid.Empty;

        var separator = field.IndexOf(':');
        if (separator <= 0)
            return false;

        return Guid.TryParse(field.AsSpan(0, separator), out channelId)
            && Guid.TryParse(field.AsSpan(separator + 1), out userId);
    }

    private static string Serialize(VoiceParticipant participant) =>
        JsonSerializer.Serialize(new StoredParticipant(participant.DisplayName, participant.JoinedAt), SerializerOptions);

    private VoiceParticipant? Deserialize(RedisValue value, Guid userId)
    {
        try
        {
            var stored = JsonSerializer.Deserialize<StoredParticipant>(value.ToString(), SerializerOptions);
            return stored is null ? null : new VoiceParticipant(userId, stored.DisplayName, stored.JoinedAt);
        }
        catch (JsonException exception)
        {
            _logger.LogWarning(exception, "Skipping malformed participant entry {Value}", value.ToString());
            return null;
        }
    }

    private sealed record StoredParticipant(string DisplayName, DateTimeOffset JoinedAt);
}
