import { CHANNEL_SERVICE_URL } from "../api";
import { authenticatedFetch } from "../authenticatedFetch";
import type { MessageParams, PaginatedItems, Message, CreateMessageRequest, UpdateMessageRequest } from "../types";

export const getMessages = async ({
    channelId,
    cursor = null,
    older = true,
    limit = 50
}: MessageParams): Promise<PaginatedItems<Message>> => {
    const params = new URLSearchParams();
    if (cursor)
        params.append('cursor', cursor);
    params.append('limit', limit.toString());
    params.append('older', older.toString());

    const response = await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/channels/${channelId}/Messages?${params.toString()}`,
        {
            method: 'GET',
        }
    );

    return response.json();
};

export const createMessage = async (
    channelId: string,
    data: CreateMessageRequest
): Promise<Message> => {
    const response = await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/channels/${channelId}/Messages`, {
        method: 'POST',
        body: JSON.stringify(data),
    });

    return response.json();
};

export const updateMessage = async (
    channelId: string,
    id: string,
    data: UpdateMessageRequest
): Promise<void> => {
    await authenticatedFetch(
        `${CHANNEL_SERVICE_URL}/api/channels/${channelId}/Messages/${id}`,
        {
            method: 'PATCH',
            body: JSON.stringify(data),
        }
    );
};

export const deleteMessage = async (channelId: string, id: string): Promise<void> => {
    await authenticatedFetch(
        `${CHANNEL_SERVICE_URL}/api/channels/${channelId}/Messages/${id}`,
        {
            method: 'DELETE',
        }
    );
};