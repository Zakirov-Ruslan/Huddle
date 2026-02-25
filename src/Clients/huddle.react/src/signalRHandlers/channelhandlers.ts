import type { QueryClient, InfiniteData } from "@tanstack/react-query";
import type { ChannelDto, ServerDto } from "../api/dtos";

export function handleCreateChannel(queryClient: QueryClient) {
    return (newChannel: ChannelDto) => {
        console.log('new channel was created (global cache handler)', newChannel);

        // Adding channel to server cache
        queryClient.setQueryData<ServerDto>(['server', newChannel.serverId], (oldServer) => {
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

        //// 2. ќбновл€ем общий кеш списка каналов, если он есть
        //queryClient.setQueryData<ChannelDto[]>(['channels'], (oldChannels) => {
        //    if (!oldChannels) return oldChannels;

        //    const channelExists = oldChannels.some(channel => channel.id === newChannel.id);
        //    if (channelExists) return oldChannels;

        //    return [...oldChannels, newChannel];
        //});

        //// 3. ќпционально: обновл€ем кеш каналов по типу, если используете такую структуру
        //queryClient.setQueryData<ChannelDto[]>(['channels', newChannel.channelType], (oldTypedChannels) => {
        //    if (!oldTypedChannels) return oldTypedChannels;

        //    const channelExists = oldTypedChannels.some(channel => channel.id === newChannel.id);
        //    if (channelExists) return oldTypedChannels;

        //    return [...oldTypedChannels, newChannel];
        //});

        // 4. Ќе забываем инвалидировать зависимые кеши, если нужно
        //queryClient.invalidateQueries({
        //    queryKey: ['serverChannels', newChannel.serverId],
        //    refetchType: 'none' // только обновление кеша, без сетевого запроса
        //});
    };
}