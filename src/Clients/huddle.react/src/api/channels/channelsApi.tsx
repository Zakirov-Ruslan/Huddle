import { CHANNEL_SERVICE_URL } from "../api";
import { authenticatedFetch } from "../authenticatedFetch";
import type { ChannelDto, CreateChannelRequest, UpdatedChannelRequest } from "../dtos";

// Создать канал
export const createChannel = async (serverId: string, data: CreateChannelRequest): Promise<ChannelDto> => {
    const response = await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/servers/${serverId}/Channels`, {
        method: 'POST',
        body: JSON.stringify(data),
    });

    return response.json();
};

// Обновить канал
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

// Удалить канал
export const deleteChannel = async (serverId: string, id: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/servers/${serverId}/Channels/${id}`, {
        method: 'DELETE',
    });
};