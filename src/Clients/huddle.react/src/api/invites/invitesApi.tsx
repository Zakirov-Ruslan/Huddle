import { CHANNEL_SERVICE_URL } from "../api";
import { authenticatedFetch } from "../authenticatedFetch";
import type { AcceptInviteResponse, InviteDto } from "../dtos";

export const getServerInvites = async (serverId: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/servers/${serverId}/invites`, {
        method: 'GET',
    });
};

export const createInvite = async (serverId: string): Promise<InviteDto> => {
    const response = await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/servers/${serverId}/invites`, {
        method: 'POST',
    });

    return response.json();
};

export const acceptInvite = async (inviteCode: string): Promise<AcceptInviteResponse> => {
    const response = await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Invites/${inviteCode}/accept`, {
        method: 'POST',
    });

    return response.json();
};

export const deleteInvite = async (inviteId: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Invites/${inviteId}`, {
        method: 'DELETE',
    });
};