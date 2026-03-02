import type { QueryClient, InfiniteData } from "@tanstack/react-query";
import type { Channel, Server } from "../api/types";


export function handleCreateChannel(queryClient: QueryClient) {
    return (newChannel: Channel) => {
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