export interface CreateChannelRequest {
    name: string;
    channelType: string;
}

export interface UpdatedChannelRequest {
    name: string;
    channelType: string;
}

export interface CreateMessageRequest {
    text: string;
}

export interface UpdateMessageRequest {
    text: string;
}

export interface CreateServerRequest {
    name: string;
    isPrivate?: boolean;
}

export interface UpdateServerRequest {
    name: string;
}

export interface UpdateMemberRequest {
    serverUsername: string;
    description: string;
}

export interface AcceptInviteResponse {
    serverId: string;
}

export interface Channel {
    id: string;
    serverId: string;
    name: string;
    channelType: string;
}

export interface Member {
    id: string;
    serverId: string;
    identityId: string;
    serverUsername: string;
    description: string;
}

export interface MessageParams
{
    channelId: string;
    cursor?: string | null;
    limit?: number;
}

export interface MembersParams {
    serverId: string;
    cursor?: string | null;
    limit?: number;
}

export interface Message {
    id: string;
    channelId: string;
    authorId: string;
    text: string;
    sentAt: Date; // ISO 8601 date-time string
    isEdited: boolean;
}
export interface PaginatedItems<T> {
    items: T[];
    hasMore: boolean;
    nextCursor: string | null;
}

export interface Server {
    id: string;
    name: string;
    ownerIdentityId: string;
    channels: Channel[];
}

export interface Invite {
    id: string;
    serverId: string;
    createdAt: Date;
    code: string;
}

export interface UserProfile {
    userId: string;
    userName: string;
}

export interface SignalRNotification<T> {
    entity: T;
    initiatorSessionId: string | null;
}