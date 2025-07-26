import { CHANNEL_SERVICE_URL } from "../api";
import { authenticatedFetch } from "../authenticatedFetch";
import type { ServerDto, UpdateServerRequest, CreateServerRequest } from "../dtos";

// Получить список серверов текущего пользователя
export const getMyServers = async (): Promise<ServerDto[]> => {
    const response = await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Servers/my`, {
        method: 'GET',
    });
    return response.json();
};

// Получить конкретный сервер
export const getServer = async (id: string): Promise<ServerDto> => {
    const response = await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Servers/${id}`, {
        method: 'GET',
    });
    return response.json();
};

// Обновить сервер
export const updateServer = async (id: string, data: UpdateServerRequest): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Servers/${id}`, {
        method: 'PUT',
        body: JSON.stringify(data),
    });
};

// Удалить сервер
export const deleteServer = async (id: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Servers/${id}`, {
        method: 'DELETE',
    });
};

// Создать сервер
export const createServer = async (data: CreateServerRequest): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Servers`, {
        method: 'POST',
        body: JSON.stringify(data),
    });
};
