import { LIVEKIT_SERVICE_URL } from "../api";
import { authenticatedFetch } from "../authenticatedFetch";
import type { ServerVoiceState } from "../types";

export const getLiveKitToken = async (serverId : string, channelId: string): Promise<string> => {

    const params = new URLSearchParams();
    params.append('serverId', serverId.toString());
    params.append('channelId', channelId.toString());

    const response = await authenticatedFetch(
        `${LIVEKIT_SERVICE_URL}/api/livekit/token?${params.toString()}`, {
        method: 'GET',
    });

    return response.json();
};

/** Who currently sits in the voice channels of the server. Grouped by channel on the server side. */
export const getServerVoiceState = async (serverId: string): Promise<ServerVoiceState> => {
    const response = await authenticatedFetch(
        `${LIVEKIT_SERVICE_URL}/api/livekit/servers/${serverId}/participants`, {
        method: 'GET',
    });

    return response.json();
};