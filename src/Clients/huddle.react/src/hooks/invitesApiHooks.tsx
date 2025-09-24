import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { AcceptInviteResponse, InviteDto } from "../api/dtos";
import { acceptInvite, createInvite } from "../api/invites/invitesApi";

export const useCreateInvite = (serverId: string) => {
    const queryClient = useQueryClient();
    return useMutation<InviteDto, Error, { serverId: string }>({
        mutationFn: () => createInvite(serverId),
        onSuccess: (createdInvite) => {
            queryClient.setQueryData<InviteDto>(['invite', serverId], (old) => {
                return createdInvite;
            });
        }
    });
};

export const useAcceptInvite = () => {
    const queryClient = useQueryClient();
    return useMutation<AcceptInviteResponse, Error, string>({
        mutationKey: ['invite', 'accept'],
        mutationFn: (inviteCode) => acceptInvite(inviteCode),
        retry: false,
        onSuccess: (acceptedInvite, inviteCode) => {
            queryClient.setQueryData<AcceptInviteResponse>(['acceptedInvite', inviteCode], acceptedInvite);
        },
    });
};