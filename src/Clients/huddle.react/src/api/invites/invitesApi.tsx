import { CHANNEL_SERVICE_URL } from "../api";
import { authenticatedFetch } from "../authenticatedFetch";

export const getServerInvites = async (serverId: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/servers/${serverId}/invites`, {
        method: 'GET',
    });
};

export const createInvite = async (serverId: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/servers/${serverId}/invites`, {
        method: 'POST',
    });
};

export const acceptInvite = async (inviteCode: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Invites/${inviteCode}/accept`, {
        method: 'POST',
    });
};

export const deleteInvite = async (inviteId: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Invites/${inviteId}`, {
        method: 'DELETE',
    });
};