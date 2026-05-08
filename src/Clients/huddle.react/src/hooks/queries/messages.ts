import { useInfiniteQuery, useMutation, useQuery, useQueryClient, type InfiniteData } from "@tanstack/react-query";
import { getMessages, createMessage, updateMessage, deleteMessage } from "../../api/messages/messagesApi";
import type { Message, CreateMessageRequest, PaginatedItems, UpdateMessageRequest } from "../../api/types";
import { useTextChannelStore } from "../../stores/textChannelStore";
import getUser from "../../utils/authHelpers";


export const useInfiniteMessages = (channelId: string) => {
    return useInfiniteQuery({
        queryKey: ['messages', channelId],
        queryFn: ({ pageParam }: { pageParam: string | null }) =>
            getMessages({
                channelId,
                cursor: pageParam
            }),
        getNextPageParam: (lastPage) => {
            return lastPage.hasMore ? lastPage.nextCursor : undefined;
        },
        initialPageParam: null,
        staleTime: 5 * 60 * 1000, //5 minutes
        refetchOnWindowFocus: false,
    });
};

export const useSendMessage = () => {
    const queryClient = useQueryClient();

    return useMutation<Message, Error, { channelId: string; data: CreateMessageRequest }, { localId: string }>({
        mutationKey: ['sendMessage'],
        mutationFn: ({ channelId, data }) => createMessage(channelId, data),
        onMutate: async (variables) => {
            const { channelId, data: message } = variables;
            const localId = `local_${Date.now()}_${Math.random().toString(36).slice(2)}`;
            const user = getUser();

            useTextChannelStore.getState().addMessage(
                channelId,
                {
                    id: localId,
                    text: message.text,
                    authorId: user.profile.sub,
                    sentAt: new Date(),
                    channelId: channelId,
                    isEdited: false
                }
            );

            return { localId };
        },
        onError: (error, variables, context) => {
            if (context?.localId) {
                useTextChannelStore.getState().updateMessageStatus(context.localId, "error", error);
            }
        },
        onSuccess: (newMessage, variables, context) => {
            const { channelId } = variables;

            if (context?.localId) {
                useTextChannelStore.getState().removeMessage(context.localId);
            }

            const currentData = queryClient.getQueryData<InfiniteData<PaginatedItems<Message>>>(['messages', newMessage.channelId]);
            const messageExists = currentData?.pages.some(page => page.items.some(msg => msg.id === newMessage.id));
            if (messageExists)
                return;

            queryClient.setQueryData<InfiniteData<PaginatedItems<Message>>>(
                ['messages', channelId],
                (oldData) => {
                    if (!oldData) {
                        return {
                            pages: [{
                                items: [newMessage],
                                hasMore: true,
                                nextCursor: null
                            }],
                            pageParams: [null]
                        };
                    }

                    const updatedPages = [...oldData.pages];
                    if (updatedPages.length > 0) {
                        updatedPages[0] = {
                            ...updatedPages[0],
                            items: [newMessage, ...updatedPages[0].items]
                        };
                    }

                    return {
                        ...oldData,
                        pages: updatedPages
                    };
                }
            );
        },
    });
};

export const useUpdateMessage = () => {
    const queryClient = useQueryClient();
    return useMutation<void, Error, { channelId: string; messageId: string; data: UpdateMessageRequest }>({
        mutationFn: ({ channelId, messageId, data }) => updateMessage(channelId, messageId, data),
        onSuccess: (_, variables) => {
            const { channelId } = variables;
            queryClient.invalidateQueries({ queryKey: ["messages", channelId] });
        },
    });
};

export const useDeleteMessage = () => {
    const queryClient = useQueryClient();
    return useMutation<void, Error, { channelId: string; messageId: string }>({
        mutationFn: ({ channelId, messageId }) => deleteMessage(channelId, messageId),
        onSuccess: (_, variables) => {
            const { channelId } = variables;
            queryClient.setQueryData<Message[]>(
                ["messages", channelId],
                (oldMessages = []) =>
                    oldMessages.filter((msg) => msg.id !== variables.messageId)
            );
        },
    });
};