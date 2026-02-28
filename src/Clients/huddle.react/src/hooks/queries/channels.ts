import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createChannel } from "../../api/channels/channelsApi";
import type { ChannelDto, CreateChannelRequest, ServerDto } from "../../api/types";

export const useCreateChannel = (serverId: string) => {
    const queryClient = useQueryClient();
    return useMutation<ChannelDto, Error, CreateChannelRequest>({
        mutationFn: (data) => createChannel(serverId, data),
        onSuccess: (createdChannel) => {
            queryClient.setQueryData<ServerDto>(['server', serverId], (oldServer) => {
                if (!oldServer)
                    return oldServer;

                // Check if already exists
                const channelExists = oldServer.channels.some(channel => channel.id === createdChannel.id);
                if (channelExists)
                    return oldServer;

                return {
                    ...oldServer,
                    channels: [...oldServer.channels, createdChannel]
                };
            });
        }
    });
};