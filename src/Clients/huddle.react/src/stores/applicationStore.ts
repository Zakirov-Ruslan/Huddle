import { create } from "zustand";
import type { Server } from "../api/types";

interface ApplicationStatus
{
    /**
     * Members panel visibility.
     * Not viewport related: on phones it is rendered as an overlay, on desktop as a column.
     */
    isMembersPanelOpened: boolean;
    toggleMembersPanelOpened: () => void;
    setMembersPanelOpened: (visible: boolean) => void;

    activeServer: Server | null;
    setActiveServer: (server: Server | null) => void;
}

export const useApplicationStore = create<ApplicationStatus>((set) => ({
    isMembersPanelOpened: false,
    toggleMembersPanelOpened: () => set((state) => ({ isMembersPanelOpened: !state.isMembersPanelOpened })),
    setMembersPanelOpened: (visible) => set({ isMembersPanelOpened: visible }),

    activeServer: null,
    setActiveServer: (server) => set({ activeServer: server }),
}))