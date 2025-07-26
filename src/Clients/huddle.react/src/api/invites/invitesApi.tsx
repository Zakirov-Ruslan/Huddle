import { CHANNEL_SERVICE_URL } from "../api";
import { authenticatedFetch } from "../authenticatedFetch";
import type { CreateInviteRequest } from "../dtos";

// Получить приглашения для сервера
export const getServerInvites = async (serverId: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/servers/${serverId}/invites`, {
        method: 'GET',
    });
};

// Создать приглашение
export const createInvite = async (serverId: string, data: CreateInviteRequest): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/servers/${serverId}/invites`, {
        method: 'POST',
        body: JSON.stringify(data),
    });
};

// Получить список ожидания приглашений текущего пользователя
export const getPendingInvites = async (): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Invites/pending`, {
        method: 'GET',
    });
};

// Принять приглашение
export const acceptInvite = async (inviteId: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Invites/${inviteId}/accept`, {
        method: 'POST',
    });
};

// Отклонить приглашение
export const declineInvite = async (inviteId: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Invites/${inviteId}/decline`, {
        method: 'POST',
    });
};

// Удалить приглашение
export const deleteInvite = async (inviteId: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Invites/${inviteId}`, {
        method: 'DELETE',
    });
};