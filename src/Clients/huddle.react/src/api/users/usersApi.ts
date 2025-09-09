import { IDENTITY_SERVICE_URL } from "../api";
import { authenticatedFetch } from "../authenticatedFetch";
import type { UserProfileDto } from "../dtos";

export const getUserProfile = async (id: string): Promise<UserProfileDto> => {
    const response = await authenticatedFetch(`${IDENTITY_SERVICE_URL}/api/UserProfiles/${id}`, {
        method: 'GET',
    });
    return response.json();
};

export const getMyProfile = async (): Promise<UserProfileDto> => {
    const response = await authenticatedFetch(`${IDENTITY_SERVICE_URL}/api/UserProfiles/me`, {
        method: 'GET',
    });
    return response.json();
};