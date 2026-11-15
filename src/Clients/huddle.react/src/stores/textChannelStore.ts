import { create } from "zustand";
import type { Message } from "../api/types";

interface MutationState {
    status: "pending" | "error" | "success",
    error?: Error;
}

export interface LocalMessage extends Message, MutationState { }

export interface ChannelScrollPosition {
    /** Message the viewport is anchored to. */
    messageId: string | null;
    /** Distance in px from the top of the list container to the anchor message. */
    offset: number;
    atBottom: boolean;
}

interface TextChannelState {
    drafts: Record<string, string>;
    setDraft: (channelId: string, text: string) => void;

    localMessages: Record<string, LocalMessage[]>;
    addMessage: (channelId: string, message: Message) => void;
    updateMessageStatus: (localId: string, status: "pending" | "error", error?: Error) => void;
    removeMessage: (localId: string) => void;

    positions: Record<string, ChannelScrollPosition>;
    setPosition: (channelId: string, position: ChannelScrollPosition) => void;
    getPosition: (channelId: string) => ChannelScrollPosition | undefined;
}

export const useTextChannelStore = create<TextChannelState>((set, get) => ({
    drafts: {},
    setDraft: (channelId, text) => set((state) => ({
        drafts: {
            ...state.drafts,
            [channelId]: text
        }
    })),

    localMessages: {},
    addMessage: (channelId, message) => {

        const localMessage: LocalMessage = {
            ...message,
            status: "pending"
        };

        set((state) => ({
            localMessages: {
                ...state.localMessages,
                [channelId]: [localMessage, ...(state.localMessages[channelId] || [])]
            }
        }));
    },

    updateMessageStatus: (localId, status, error) => {
        set((state) => {
            const newLocalMessages = { ...state.localMessages };

            for (const channelId in newLocalMessages) {
                const index = newLocalMessages[channelId].findIndex(m => m.id === localId);
                if (index !== -1) {
                    const updatedMessages = [...newLocalMessages[channelId]];
                    updatedMessages[index] = {
                        ...updatedMessages[index],
                        status,
                        error
                    };
                    newLocalMessages[channelId] = updatedMessages;
                    break;
                }
            }

            return { localMessages: newLocalMessages };
        });
    },

    removeMessage: (localId) => {
        set((state) => {
            const newLocalMessages = { ...state.localMessages };

            for (const channelId in newLocalMessages) {
                newLocalMessages[channelId] = newLocalMessages[channelId].filter(m => m.id !== localId);
                if (newLocalMessages[channelId].length === 0) {
                    delete newLocalMessages[channelId];
                }
            }

            return { localMessages: newLocalMessages };
        });
    },

    positions: {},
    setPosition: (channelId, position) => set((state) => {
        const current = state.positions[channelId];

        if (current
            && current.messageId === position.messageId
            && current.offset === position.offset
            && current.atBottom === position.atBottom)
            return state;

        return {
            positions: {
                ...state.positions,
                [channelId]: position
            }
        };
    }),
    getPosition: (channelId) => get().positions[channelId]
}));
