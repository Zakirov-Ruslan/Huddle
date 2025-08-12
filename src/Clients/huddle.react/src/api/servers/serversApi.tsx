import { CHANNEL_SERVICE_URL } from "../api";
import { authenticatedFetch } from "../authenticatedFetch";
import type { ServerDto, UpdateServerRequest, CreateServerRequest } from "../dtos";

export const getMyServers = async (): Promise<ServerDto[]> => {
    const response = await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Servers/my`, {
        method: 'GET',
    });
    return response.json();
};

export const getServer = async (id: string): Promise<ServerDto> => {
    const response = await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Servers/${id}`, {
        method: 'GET',
    });
    return response.json();
};

export const updateServer = async (id: string, data: UpdateServerRequest): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Servers/${id}`, {
        method: 'PUT',
        body: JSON.stringify(data),
    });
};

export const deleteServer = async (id: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Servers/${id}`, {
        method: 'DELETE',
    });
};

export const createServer = async (data: CreateServerRequest): Promise<ServerDto> => {
    const response = await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Servers`, {
        method: 'POST',
        body: JSON.stringify(data),
    });

    return response.json();
};
