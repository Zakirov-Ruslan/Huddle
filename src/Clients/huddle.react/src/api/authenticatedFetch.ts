import getUser from "../utils/authHelpers";

export const authenticatedFetch = async (url: string, options: RequestInit = {}) => {
    const user = getUser();
    const token = user?.access_token;

    if (!token)
        throw new Error('No token provided');

    const defaultHeaders: HeadersInit = {
        'Content-Type': 'application/json',
    };

    defaultHeaders['Authorization'] = `Bearer ${token}`;

    const config: RequestInit = {
        ...options,
        headers: {
        ...defaultHeaders,
        ...options.headers,
        },
    };

    const response = await fetch(url, config);

    if (!response.ok)
        throw new Error(`HTTP error! status: ${response.status}, message: ${await response.text() }`);

    return response;
};