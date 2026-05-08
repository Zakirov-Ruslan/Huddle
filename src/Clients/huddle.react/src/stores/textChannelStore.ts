import { create } from "zustand";

interface TextChannelState
{
    drafts: Record<string, string>;
    setDraft: (channelId: string, text: string) => void;
}

export const useTextChannelStore = create<TextChannelState>((set) => ({
    drafts: {},
    
    setDraft: (channelId, text) => set((state) => ({
        drafts: {
            ...state.drafts,
            [channelId]: text
        }
    })),
}));