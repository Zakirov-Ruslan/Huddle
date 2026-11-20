import type { QueryClient } from "@tanstack/react-query";
import type {
    ChannelVoiceState,
    ServerVoiceState,
    VoiceChannelClosedNotification,
    VoiceParticipant,
    VoiceParticipantJoinedNotification,
    VoiceParticipantLeftNotification,
} from "../api/types";

/**
 * Applies a voice push to the cached snapshot of a server.
 *
 * The pushes are deltas, and the version tells whether a delta still belongs to the snapshot the client
 * holds: everything at or below `state.version` is already part of that snapshot (or is an older push
 * that overtook a newer one), so it must not be applied again.
 */
const applyUpdate = (
    queryClient: QueryClient,
    serverId: string,
    version: number,
    update: (channels: ChannelVoiceState[]) => ChannelVoiceState[]
) => {
    const snapshot = queryClient.getQueryData<ServerVoiceState>(['voiceState', serverId]);

    // No snapshot means the push either overtook the first request of a page that just opened or arrived
    // for a server nobody is looking at. Re-reading the state is cheaper and safer than assembling the
    // list from deltas; an absent cache is exactly the case where that would produce a partial list.
    if (!snapshot) {
        void queryClient.invalidateQueries({ queryKey: ['voiceState', serverId] });
        return;
    }

    if (version <= snapshot.version)
        return;

    queryClient.setQueryData<ServerVoiceState>(['voiceState', serverId], (state) => {
        if (!state || version <= state.version)
            return state;

        return { ...state, version, channels: update(state.channels) };
    });
};

/** Adds the participant, replacing a previous entry of the same user so duplicate pushes stay idempotent. */
const withParticipant = (
    channels: ChannelVoiceState[],
    notification: VoiceParticipantJoinedNotification
): ChannelVoiceState[] => {
    const participant: VoiceParticipant = {
        userId: notification.userId,
        displayName: notification.displayName,
        joinedAt: notification.joinedAt,
    };

    if (!channels.some(channel => channel.channelId === notification.channelId))
        return [...channels, { channelId: notification.channelId, participants: [participant] }];

    return channels.map(channel => channel.channelId !== notification.channelId
        ? channel
        : {
            ...channel,
            participants: [
                ...channel.participants.filter(entry => entry.userId !== participant.userId),
                participant,
            ],
        });
};

/** Removes the participant and drops the channel once nobody is left in it, matching the snapshot shape. */
const withoutParticipant = (channels: ChannelVoiceState[], channelId: string, userId: string): ChannelVoiceState[] =>
    channels
        .map(channel => channel.channelId !== channelId
            ? channel
            : { ...channel, participants: channel.participants.filter(entry => entry.userId !== userId) })
        .filter(channel => channel.participants.length > 0);

export const handleVoiceParticipantJoined = (queryClient: QueryClient) => {
    return (notification: VoiceParticipantJoinedNotification) => {
        console.debug('voice participant joined (global cache handler)', notification);

        applyUpdate(queryClient, notification.serverId, notification.version, channels =>
            withParticipant(channels, notification));
    };
};

export const handleVoiceParticipantLeft = (queryClient: QueryClient) => {
    return (notification: VoiceParticipantLeftNotification) => {
        console.debug('voice participant left (global cache handler)', notification);

        applyUpdate(queryClient, notification.serverId, notification.version, channels =>
            withoutParticipant(channels, notification.channelId, notification.userId));
    };
};

export const handleVoiceChannelClosed = (queryClient: QueryClient) => {
    return (notification: VoiceChannelClosedNotification) => {
        console.debug('voice channel closed (global cache handler)', notification);

        applyUpdate(queryClient, notification.serverId, notification.version, channels =>
            channels.filter(channel => channel.channelId !== notification.channelId));
    };
};
