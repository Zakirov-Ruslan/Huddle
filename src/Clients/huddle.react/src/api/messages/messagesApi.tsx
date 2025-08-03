import { CHANNEL_SERVICE_URL } from "../api";
import { authenticatedFetch } from "../authenticatedFetch";
import type { MessageDto, CreateMessageRequest, UpdateMessageRequest } from "../dtos";

// Получить сообщения канала (новые)
export const getChannelMessages = async (
    channelId: string,
    pageSize: number = 50
): Promise<MessageDto[]> => {
    const response = await authenticatedFetch(
        `${CHANNEL_SERVICE_URL}/api/channels/${channelId}/Messages?pageSize=${pageSize}`,
        {
            method: 'GET',
        }
    );
    return response.json();
};

// Получить более старые сообщения
export const getOlderMessages = async (
    channelId: string,
    beforeMessageId?: string,
    pageSize: number = 50
): Promise<MessageDto[]> => {
    let url = `${CHANNEL_SERVICE_URL}/api/channels/${channelId}/Messages/older?pageSize=${pageSize}`;
    if (beforeMessageId) {
        url += `&beforeMessageId=${beforeMessageId}`;
    }
    const response = await authenticatedFetch(url, {
        method: 'GET',
    });
    return response.json();
};

// Создать сообщение
export const createMessage = async (
    channelId: string,
    data: CreateMessageRequest
): Promise<MessageDto> => {
    const response = await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/channels/${channelId}/Messages`, {
        method: 'POST',
        body: JSON.stringify(data),
    });

    return response.json();
};

// Обновить сообщение
export const updateMessage = async (
    channelId: string,
    id: string,
    data: UpdateMessageRequest
): Promise<void> => {
    await authenticatedFetch(
        `${CHANNEL_SERVICE_URL}/api/channels/${channelId}/Messages/${id}`,
        {
            method: 'PATCH', // API использует PATCH
            body: JSON.stringify(data),
        }
    );
};

// Удалить сообщение
export const deleteMessage = async (channelId: string, id: string): Promise<void> => {
    await authenticatedFetch(
        `${CHANNEL_SERVICE_URL}/api/channels/${channelId}/Messages/${id}`,
        {
            method: 'DELETE',
        }
    );
};