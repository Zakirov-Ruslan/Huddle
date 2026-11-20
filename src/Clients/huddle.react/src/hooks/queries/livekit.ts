import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import { useAuth } from "react-oidc-context";
import { getLiveKitToken, getServerVoiceState } from "../../api/liveKit/liveKitApi";
import { useSignalRState } from "../../providers/SignalRProvider";
import { useMyProfile } from "./users";
import { useVoiceChannelStore } from "../../stores/voiceChannelStore";
import type { ServerVoiceState, VoiceParticipant } from "../../api/types";

export const useLiveKitToken = (serverId: string, channelId: string) => {
    return useQuery<string, Error>({
        queryKey: ['liveKitToken', channelId],
        queryFn: () => getLiveKitToken(serverId, channelId),

        staleTime: 10 * 60 * 1000
    });
};

/**
 * Adds the local user to the channel they are connected to.
 *
 * LiveKit reports the local participant to the client immediately, while the snapshot reaches the server
 * only after the webhook round-trip (and the first snapshot of a page can easily be older than the join).
 * Merging the known local state keeps the first join from looking broken without touching the version:
 * the next push or snapshot supersedes it as usual.
 */
const withOwnParticipant = (
    state: ServerVoiceState,
    channelId: string,
    userId: string,
    displayName: string
): ServerVoiceState => {
    const channel = state.channels.find(entry => entry.channelId === channelId);

    if (channel?.participants.some(participant => participant.userId === userId))
        return state;

    const ownParticipant: VoiceParticipant = { userId, displayName, joinedAt: new Date().toISOString() };

    return {
        ...state,
        channels: channel
            ? state.channels.map(entry => entry.channelId !== channelId
                ? entry
                : { ...entry, participants: [ownParticipant, ...entry.participants] })
            : [...state.channels, { channelId, participants: [ownParticipant] }],
    };
};

/**
 * Voice presence of a server, keyed by `['voiceState', serverId]`.
 *
 * The updates arrive as `VoiceParticipantJoined` / `VoiceParticipantLeft` pushes and are applied to this
 * cache entry (see `signalRHandlers/voice`). The short lifetime keeps an unnoticed gap from surviving for
 * long: a mount or a window focus refetches the snapshot. While the hub is disconnected there is nothing
 * to patch the cache with, so the query falls back to polling until the connection is back.
 */
export const useServerVoiceState = (serverId: string | undefined) => {
    const { isConnected } = useSignalRState();
    const { activeChannel } = useVoiceChannelStore();
    const { data: profile } = useMyProfile();
    const auth = useAuth();

    const ownUserId = auth.user?.profile.sub;

    const query = useQuery<ServerVoiceState, Error>({
        queryKey: ['voiceState', serverId],
        queryFn: () => getServerVoiceState(serverId!),
        enabled: !!serverId,

        staleTime: 15 * 1000,
        refetchInterval: isConnected ? false : 30 * 1000,
        refetchOnWindowFocus: true,
    });

    const data = useMemo(() => {
        if (!query.data || !serverId || !ownUserId || activeChannel?.serverId !== serverId)
            return query.data;

        return withOwnParticipant(query.data, activeChannel.id, ownUserId, profile?.userName ?? "");
    }, [query.data, serverId, ownUserId, activeChannel, profile?.userName]);

    return { ...query, data };
};
