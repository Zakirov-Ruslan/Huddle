import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createChannel } from "../api/channels/channelsApi";
import type { ChannelDto, CreateChannelRequest, ServerDto } from "../api/dtos";


// Создать сервер
export const useCreateChannel = (serverId: string) => {
    const queryClient = useQueryClient();
    return useMutation<ChannelDto, Error, CreateChannelRequest>({
        mutationFn: (data) => createChannel(serverId, data),
        onSuccess: (createdChannel) => {
            queryClient.setQueryData<ServerDto>(['server', serverId], (old) => {
                if (!old) return old;
                return {
                    ...old,
                    channels: [...old.channels, createdChannel]
                };
            });
        }
    });
};