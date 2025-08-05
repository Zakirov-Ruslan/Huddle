export interface ChannelDto {
  id: string;
  serverId: string;
  name: string;
  channelType: string;
}

export interface CreateChannelRequest {
  name: string;
  channelType: string;
}

export interface UpdatedChannelRequest {
  name: string;
  channelType: string;
}

export interface CreateInviteRequest {
  userId: string;
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

export interface MemberDto {
  id: string;
  serverId: string;
  identityId: string;
  serverUsername: string;
  description: string;
}

export interface UpdateMemberRequest {
  serverUsername: string;
  description: string;
}

export interface MessageDto {
  id: string;
  channelId: string;
  authorId: string;
  text: string;
  sentAt: Date; // ISO 8601 date-time string
  isEdited: boolean;
}

export interface ServerDto {
  id: string;
  name: string;
  channels: ChannelDto[];
}