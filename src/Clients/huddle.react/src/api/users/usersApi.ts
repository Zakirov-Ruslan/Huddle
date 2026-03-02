import { IDENTITY_SERVICE_URL } from "../api";
import { authenticatedFetch } from "../authenticatedFetch";
import type { UserProfile } from "../types";

export const getUserProfile = async (id: string): Promise<UserProfile> => {
    const response = await authenticatedFetch(`${IDENTITY_SERVICE_URL}/api/UserProfiles/${id}`, {
        method: 'GET',
    });
    return response.json();
};

export const getMyProfile = async (): Promise<UserProfile> => {
    const response = await authenticatedFetch(`${IDENTITY_SERVICE_URL}/api/UserProfiles/me`, {
        method: 'GET',
    });
    return response.json();
};