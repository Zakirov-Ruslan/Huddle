import { CHANNEL_SERVICE_URL } from "../api";
import { authenticatedFetch } from "../authenticatedFetch";
import type { MemberDto, MembersParams, PaginatedItems, UpdateMemberRequest } from "../types";

export const getMember = async (serverId: string, memberId: string): Promise<MemberDto> => {
    const response = await authenticatedFetch(
        `${CHANNEL_SERVICE_URL}/api/servers/${serverId}/Members/${memberId}`, {
            method: 'GET',
        }
    );
    return response.json();
};

export const getServerMembers = async ({ serverId, cursor = null, limit = 50 }: MembersParams): Promise<PaginatedItems<MemberDto>> => {

    const params = new URLSearchParams();
    if (cursor) params.append('cursor', cursor);
    params.append('limit', limit.toString());

    const response = await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/servers/${serverId}/Members?${params.toString()}`, {
            method: 'GET',
        }
    );

    return response.json();
};

export const joinServer = async (serverId: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/server/${serverId}/join`, {
        method: 'POST',
    });
};

export const updateMember = async (memberId: string, data: UpdateMemberRequest): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Members/${memberId}`, {
        method: 'PUT',
        body: JSON.stringify(data),
    });
};

export const deleteMember = async (memberId: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Members/${memberId}`, {
        method: 'DELETE',
    });
};