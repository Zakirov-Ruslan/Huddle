import { create } from "zustand";
import type { Server } from "../api/types";

interface ApplicationStatus
{
    isMembersPanelOpened: boolean,
    toggleMembersPanedOpened: () => void;

    activeServer: Server | null; 
    setActiveServer: (server: Server | undefined) => void;
}

export const useApplicationStore = create<ApplicationStatus>((set) => ({
    isMembersPanelOpened: false,
    toggleMembersPanedOpened: () => set((state) => ({isMembersPanelOpened: !state.isMembersPanelOpened})),

    activeServer: null,
    setActiveServer: (server) => set({ activeServer: server }),
}))