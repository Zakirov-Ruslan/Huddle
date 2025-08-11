import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { ChannelDto, CreateChannelRequest, ServerDto } from "../dtos";
import { createChannel } from "./channelsApi";

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