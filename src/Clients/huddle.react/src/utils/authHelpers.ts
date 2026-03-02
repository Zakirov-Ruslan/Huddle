import { User } from "oidc-client-ts";

export default function getUser(): User {
    const identityUrl = import.meta.env.VITE_IDENTITY_URL;
    const storageUserString = `oidc.user:${identityUrl}:interactive.confidential`;
    const oidcStorage = sessionStorage.getItem(storageUserString);

    if (!oidcStorage)
        throw new Error('No user data in session storage');

    const user = User.fromStorageString(oidcStorage);
    return user;
}

const SESSION_STORAGE_KEY = 'sessionId';

export const initSessionId = (): string => {
    const sessionId = crypto.randomUUID();
    sessionStorage.setItem(SESSION_STORAGE_KEY, sessionId);
    return sessionId;
};

export const getSessionId = (): string | null => {
    return sessionStorage.getItem(SESSION_STORAGE_KEY);
};