import { CHANNEL_SERVICE_URL } from "../api";
import { authenticatedFetch } from "../authenticatedFetch";

export const getLiveKitToken = async (serverId : string, channelId: string): Promise<string> => {

    const params = new URLSearchParams();
    params.append('serverId', serverId.toString());
    params.append('channelId', channelId.toString());

    const response = await authenticatedFetch(
        `${CHANNEL_SERVICE_URL}/api/livekit/token?${params.toString()}`, {
        method: 'GET',
    });

    return response.json();
};