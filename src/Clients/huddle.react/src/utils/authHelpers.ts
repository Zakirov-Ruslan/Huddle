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