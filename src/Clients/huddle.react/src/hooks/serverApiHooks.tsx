import { useQueryClient, useMutation, useQuery } from "@tanstack/react-query";
import type { CreateServerRequest, ServerDto, UpdateServerRequest } from "../api/dtos";
import { updateServer, deleteServer, createServer, getMyServers, getServer } from "../api/servers/serversApi";

 export const useServer = (serverId: string) => {
    return useQuery<ServerDto, Error>({
        queryKey: ['server', serverId],
        queryFn: () => getServer(serverId),
    });
};

export const useMyServers = () => {
    return useQuery<ServerDto[], Error>({
        queryKey: ['myServers'],
        queryFn: getMyServers,
    });
};

export const useCreateServer = () => {
    const queryClient = useQueryClient();
    return useMutation<ServerDto, Error, CreateServerRequest>({
        mutationFn: createServer,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['myServers'] });
        },
    });
};

export const useUpdateServer = () => {
    const queryClient = useQueryClient();
    return useMutation<void, Error, { id: string; data: UpdateServerRequest }>({
        mutationFn: ({ id, data }) => updateServer(id, data),
        onSuccess: (variables) => { 
            queryClient.invalidateQueries({ queryKey: ['server', variables.id] });
            queryClient.invalidateQueries({ queryKey: ['myServers'] });
        },
    });
};

export const useDeleteServer = () => {
    const queryClient = useQueryClient();
    return useMutation<void, Error, string>({
        mutationFn: deleteServer,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['myServers'] });
        },
    });
};