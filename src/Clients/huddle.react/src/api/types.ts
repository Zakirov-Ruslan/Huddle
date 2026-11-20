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
    attachmentIds?: string[];
}

export interface UpdateMessageRequest {
    text: string;
}

export interface CreateServerRequest {
    name: string;
    isPrivate?: boolean;
    avatarFileId?: string | null;
}

export interface UpdateServerRequest {
    name: string;
    avatarFileId?: string | null;
    removeAvatar?: boolean;
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
    older: boolean;
}

export interface MembersParams {
    serverId: string;
    cursor?: string | null;
    limit?: number;
}

export interface MessageAttachment {
    fileId: string;
    fileName: string;
    contentType: string;
    size: number;
    isImage: boolean;
    /** Relative path served by the file service, e.g. `/files/api/files/{id}/content`. */
    url: string;
}

export interface Message {
    id: string;
    channelId: string;
    authorId: string;
    text: string;
    sentAt: Date; // ISO 8601 date-time string
    isEdited: boolean;
    attachments?: MessageAttachment[];
}
export interface PaginatedItems<T> {
    items: T[];
    hasNext: boolean; // newer 
    hasPrev: boolean; // older
    nextCursor: string | null;
    prevCursor: string | null;
}

export interface Server {
    id: string;
    name: string;
    ownerIdentityId: string;
    avatarUrl?: string | null;
    channels: Channel[];
}

export interface VoiceParticipant {
    userId: string;
    displayName: string;
    joinedAt: string;
}

/** Voice participants of a single channel. Channels without participants are not sent by the API. */
export interface ChannelVoiceState {
    channelId: string;
    participants: VoiceParticipant[];
}

/**
 * Snapshot of who sits in the voice channels of a server.
 * `version` grows with every change and orders the snapshot against the SignalR pushes.
 */
export interface ServerVoiceState {
    serverId: string;
    version: number;
    channels: ChannelVoiceState[];
}

/** Payload of the `VoiceParticipantJoined` notification. */
export interface VoiceParticipantJoinedNotification {
    serverId: string;
    channelId: string;
    userId: string;
    displayName: string;
    joinedAt: string;
    version: number;
}

/** Payload of the `VoiceParticipantLeft` notification. */
export interface VoiceParticipantLeftNotification {
    serverId: string;
    channelId: string;
    userId: string;
    reason: string;
    version: number;
}

/** Payload of the `VoiceChannelClosed` notification. */
export interface VoiceChannelClosedNotification {
    serverId: string;
    channelId: string;
    version: number;
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