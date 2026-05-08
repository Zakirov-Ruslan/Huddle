import { create } from "zustand";
import type { Message } from "../api/types";

interface MutationState {
    status: "pending" | "error" | "success",
    error?: Error;
}

export interface LocalMessage extends Message, MutationState { }

interface TextChannelState {
    drafts: Record<string, string>;
    setDraft: (channelId: string, text: string) => void;

    localMessages: Record<string, LocalMessage[]>;
    addMessage: (channelId: string, message: Message) => void;
    updateMessageStatus: (localId: string, status: "pending" | "error", error?: Error) => void;
    removeMessage: (localId: string) => void;
}

export const useTextChannelStore = create<TextChannelState>((set) => ({
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
    }
}));