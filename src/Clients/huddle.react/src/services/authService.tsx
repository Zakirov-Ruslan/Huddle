//import { UserManager } from 'oidc-client-ts';
//const identityUrl = import.meta.env.VITE_IDENTITY_URL;

//export const config = {
//    authority: identityUrl,
//    client_id: 'interactive.confidential',
//    redirect_uri: `${window.location.origin}/callback`,
//    response_type: 'code',
//    scope: 'openid profile api1 offline_access',
//    post_logout_redirect_uri: window.location.origin,
//};

//const userManager = new UserManager(config);

//export function signinRedirect() {
//    return userManager.signinRedirect();
//}

//export function signinRedirectCallback() {
//    return userManager.signinRedirectCallback();
//}

//export function signoutRedirect() {
//    return userManager.signoutRedirect();
//}

//export function getUser() {
//    return userManager.getUser();
//}

//export function isAuthenticated() {
//    const value = localStorage.getItem('user');
//    if (value == null)
//        return false;
//    const user = JSON.parse(value);
//    return !!(user && user.access_token);
//}