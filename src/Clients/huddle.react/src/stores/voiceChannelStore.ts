import { create } from "zustand";

interface VoiceChannelState {
    activeChannel: {
        id: string;
        serverId: string;
        name: string;
    } | null;
    token: string | undefined;

    joinChannel: (channel: { id: string; serverId: string; name: string }, token: string) => void;
    leaveChannel: () => void;
}

export const useVoiceChannelStore = create<VoiceChannelState>((set) => ({
    activeChannel: null,
    token: undefined,

    joinChannel: (channel, token) => set({ activeChannel: channel, token }),
    leaveChannel: () => set({ activeChannel: null, token: undefined }),
}));