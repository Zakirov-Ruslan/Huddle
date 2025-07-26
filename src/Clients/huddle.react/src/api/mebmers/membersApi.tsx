import { CHANNEL_SERVICE_URL } from "../api";
import { authenticatedFetch } from "../authenticatedFetch";
import type { MemberDto, UpdateMemberRequest } from "../dtos";

// ѕолучить конкретного участника
export const getMember = async (serverId: string, memberId: string): Promise<MemberDto> => {
    const response = await authenticatedFetch(
        `${CHANNEL_SERVICE_URL}/api/servers/${serverId}/Members/${memberId}`,
        {
            method: 'GET',
        }
    );
    return response.json();
};

// ѕолучить список участников сервера
export const getServerMembers = async (serverId: string): Promise<MemberDto[]> => {
    const response = await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/servers/${serverId}/Members`, {
        method: 'GET',
    });
    return response.json();
};

// ѕрисоединитьс€ к серверу
export const joinServer = async (serverId: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/server/${serverId}/join`, {
        method: 'POST',
    });
};

// ќбновить участника
export const updateMember = async (memberId: string, data: UpdateMemberRequest): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Members/${memberId}`, {
        method: 'PUT',
        body: JSON.stringify(data),
    });
};

// ”далить участника (покинуть сервер или удалить другого участника?)
export const deleteMember = async (memberId: string): Promise<void> => {
    await authenticatedFetch(`${CHANNEL_SERVICE_URL}/api/Members/${memberId}`, {
        method: 'DELETE',
    });
};