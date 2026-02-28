import { useMutation, useQueryClient } from "@tanstack/react-query";
import { acceptInvite, createInvite } from "../../api/invites/invitesApi";
import type { InviteDto, AcceptInviteResponse } from "../../api/types";

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
    return useMutation<AcceptInviteResponse, Error, { inviteCode: string; requestId: string }>({
        mutationKey: ['invite', 'accept'],
        mutationFn: ({ inviteCode, requestId }) => acceptInvite(inviteCode, requestId),
        retry: false,
        onSuccess: (acceptedInvite, inviteCode) => {
            queryClient.setQueryData<AcceptInviteResponse>(['acceptedInvite', inviteCode], acceptedInvite);
        },
    });
};