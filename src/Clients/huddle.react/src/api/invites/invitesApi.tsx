import { CHANNEL_SERVICE_URL } from "../api";
import { authenticatedFetch } from "../authenticatedFetch";
import type { CreateInviteRequest } from "../dtos";

export const getServerInvites = async (serverId: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/servers/${serverId}/invites`, {
        method: 'GET',
    });
};

export const createInvite = async (serverId: string, data: CreateInviteRequest): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/servers/${serverId}/invites`, {
        method: 'POST',
        body: JSON.stringify(data),
    });
};

export const getPendingInvites = async (): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Invites/pending`, {
        method: 'GET',
    });
};

export const acceptInvite = async (inviteId: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Invites/${inviteId}/accept`, {
        method: 'POST',
    });
};

export const declineInvite = async (inviteId: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Invites/${inviteId}/decline`, {
        method: 'POST',
    });
};

export const deleteInvite = async (inviteId: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Invites/${inviteId}`, {
        method: 'DELETE',
    });
};