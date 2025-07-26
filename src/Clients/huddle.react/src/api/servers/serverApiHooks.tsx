import { useQueryClient, useMutation, useQuery, type UseMutationOptions, type UseQueryOptions } from "@tanstack/react-query";
import type { CreateServerRequest, ServerDto, UpdateServerRequest } from "../dtos";
import { updateServer, deleteServer, createServer, getMyServers, getServer } from "./serversApi";

// Получить конкретный сервер
 export const useServer = (serverId: string) => {
    return useQuery<ServerDto, Error>({
        queryKey: ['server', serverId], // Уникальный ключ, включающий ID
        queryFn: () => getServer(serverId),
        // enabled: !!serverId, // Опционально: отключить запрос, если serverId пуст
    });
};

// Получить список своих серверов
export const useMyServers = () => {
    return useQuery<ServerDto[], Error>({
        queryKey: ['myServers'],
        queryFn: getMyServers,
    });
};

// Создать сервер
export const useCreateServer = () => {
    const queryClient = useQueryClient();
    return useMutation<void, Error, CreateServerRequest>({
        mutationFn: createServer,
        onSuccess: () => {
            // Инвалидируем кэш списка серверов, чтобы получить обновленные данные
            queryClient.invalidateQueries({ queryKey: ['myServers'] });
        },
    });
};

// Обновить сервер
export const useUpdateServer = () => {
    const queryClient = useQueryClient();
    return useMutation<void, Error, { id: string; data: UpdateServerRequest }>({
        mutationFn: ({ id, data }) => updateServer(id, data),
        onSuccess: (variables) => { // variables - это аргументы, переданные в mutate()
            queryClient.invalidateQueries({ queryKey: ['server', variables.id] });
            queryClient.invalidateQueries({ queryKey: ['myServers'] });
        },
    });
};

// Удалить сервер
export const useDeleteServer = () => {
    const queryClient = useQueryClient();
    return useMutation<void, Error, string>({ // string - это тип serverId
        mutationFn: deleteServer,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['myServers'] });
            // Возможно, также нужно удалить кэш отдельного сервера, если он был
            // queryClient.removeQueries({ queryKey: ['server'] }); // или более конкретно
        },
    });
};