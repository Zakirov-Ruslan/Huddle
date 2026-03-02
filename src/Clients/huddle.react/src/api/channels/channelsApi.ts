import { CHANNEL_SERVICE_URL } from "../api";
import { authenticatedFetch } from "../authenticatedFetch";
import type { Channel, CreateChannelRequest, UpdatedChannelRequest } from "../types";

export const createChannel = async (serverId: string, data: CreateChannelRequest): Promise<Channel> => {
    const response = await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/servers/${serverId}/Channels`, {
        method: 'POST',
        body: JSON.stringify(data),
    });

    return response.json();
};

export const updateChannel = async (
    serverId: string,
    id: string,
    data: UpdatedChannelRequest
): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/servers/${serverId}/Channels/${id}`, {
        method: 'PUT',
        body: JSON.stringify(data),
    });
};

export const deleteChannel = async (serverId: string, id: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/servers/${serverId}/Channels/${id}`, {
        method: 'DELETE',
    });
};