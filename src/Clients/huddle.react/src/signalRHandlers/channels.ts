import type { QueryClient } from "@tanstack/react-query";
import type { Channel, Server, SignalRNotification } from "../api/types";

export function handleCreateChannel(queryClient: QueryClient) {
    return (notification: SignalRNotification<Channel>) => {
        const newChannel = notification.entity;
        console.debug('new channel was created (global cache handler)', newChannel);

        // Adding channel to server cache
        queryClient.setQueryData<Server>(['server', newChannel.serverId], (oldServer) => {
            if (!oldServer)
                return oldServer;

            // Check if already exists
            const channelExists = oldServer.channels.some(channel => channel.id === newChannel.id);
            if (channelExists)
                return oldServer;

            return {
                ...oldServer,
                channels: [...oldServer.channels, newChannel]
            };
        });
    };
}