import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { createMessage, deleteMessage, getChannelMessages, getOlderMessages, updateMessage } from "./messagesApi";
import type { CreateMessageRequest, MessageDto, UpdateMessageRequest } from "../dtos";

// TODO: useInfiniteQuery 

// Получить последние сообщения канала
export const useChannelMessages = (channelId: string, pageSize: number = 50) => {
    return useQuery<MessageDto[], Error>({
        queryKey: ["channelMessages", channelId],
        queryFn: () => getChannelMessages(channelId, pageSize),
        enabled: !!channelId,
    });
};

// Получить более старые сообщения (бесконечный скролл вверх)
export const useGetOlderMessages = (channelId: string, beforeMessageId?: string, pageSize: number = 50) => {
    return useQuery<MessageDto[], Error>({
        queryKey: ["olderMessages", channelId, beforeMessageId],
        queryFn: () => getOlderMessages(channelId, beforeMessageId, pageSize),
        enabled: !!channelId,
    });
};

// Отправить сообщение
export const useSendMessage = () => {
    const queryClient = useQueryClient();
    return useMutation<MessageDto, Error, { channelId: string; data: CreateMessageRequest }>({
        mutationFn: ({ channelId, data }) => createMessage(channelId, data),
        onSuccess: (newMessage, variables) => {
            const { channelId } = variables;
            // Обновляем кэш текущего канала
            queryClient.setQueryData<MessageDto[]>(
                ["channelMessages", channelId],
                (oldMessages = []) => [newMessage, ...oldMessages]
            );
        },
    });
};

// Обновить сообщение
export const useUpdateMessage = () => {
    const queryClient = useQueryClient();
    return useMutation<void, Error, { channelId: string; messageId: string; data: UpdateMessageRequest }>({
        mutationFn: ({ channelId, messageId, data }) => updateMessage(channelId, messageId, data),
        onSuccess: (_, variables) => {
            const { channelId } = variables;
            // Инвалидируем кэш сообщений, чтобы подтянулись обновлённые данные
            queryClient.invalidateQueries({ queryKey: ["channelMessages", channelId] });
            queryClient.invalidateQueries({ queryKey: ["olderMessages", channelId] });
        },
    });
};

// Удалить сообщение
export const useDeleteMessage = () => {
    const queryClient = useQueryClient();
    return useMutation<void, Error, { channelId: string; messageId: string }>({
        mutationFn: ({ channelId, messageId }) => deleteMessage(channelId, messageId),
        onSuccess: (_, variables) => {
            const { channelId } = variables;
            // Удаляем сообщение из кэша
            queryClient.setQueryData<MessageDto[]>(
                ["channelMessages", channelId],
                (oldMessages = []) =>
                    oldMessages.filter((msg) => msg.id !== variables.messageId)
            );
            queryClient.setQueryData<MessageDto[]>(
                ["olderMessages", channelId],
                (oldMessages = []) =>
                    oldMessages.filter((msg) => msg.id !== variables.messageId)
            );
        },
    });
};