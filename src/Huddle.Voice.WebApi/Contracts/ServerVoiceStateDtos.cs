using Huddle.Voice.WebApi.Voice;

namespace Huddle.Voice.WebApi.Contracts;

/// <summary>Response of <c>GET api/livekit/servers/{serverId}/participants</c>.</summary>
public sealed record ServerVoiceStateDto(
    Guid ServerId,
    long Version,
    IReadOnlyList<ChannelVoiceStateDto> Channels);

/// <summary>Voice participants of one channel. Channels without participants are not sent.</summary>
public sealed record ChannelVoiceStateDto(
    Guid ChannelId,
    IReadOnlyList<VoiceParticipantDto> Participants);

public sealed record VoiceParticipantDto(
    Guid UserId,
    string DisplayName,
    DateTimeOffset JoinedAt);

public static class VoiceStateMapping
{
    public static ServerVoiceStateDto ToDto(this ServerVoiceStateSnapshot snapshot, Guid serverId) =>
        new(
            serverId,
            snapshot.Version,
            snapshot.Channels
                .Select(channel => new ChannelVoiceStateDto(
                    channel.ChannelId,
                    channel.Participants
                        .Select(participant => new VoiceParticipantDto(
                            participant.UserId,
                            participant.DisplayName,
                            participant.JoinedAt))
                        .ToArray()))
                .ToArray());
}
