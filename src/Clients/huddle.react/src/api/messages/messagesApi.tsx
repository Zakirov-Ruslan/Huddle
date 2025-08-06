import { CHANNEL_SERVICE_URL } from "../api";
import { authenticatedFetch } from "../authenticatedFetch";
import type { MessageDto, CreateMessageRequest, UpdateMessageRequest, PaginatedItems, MessageParams } from "../dtos";

// Получить сообщения канала (новые)
export const getMessages = async ({
    channelId,
    cursor = null,
    limit = 50
}: MessageParams): Promise<PaginatedItems<MessageDto>> => {
    const params = new URLSearchParams();
    if (cursor) params.append('cursor', cursor);
    params.append('limit', limit.toString());

    const response = await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/channels/${channelId}/Messages?${params.toString()}`,
        {
            method: 'GET',
        }
    );

    if (!response.ok) {
        throw new Error('Failed to fetch messages');
    }

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