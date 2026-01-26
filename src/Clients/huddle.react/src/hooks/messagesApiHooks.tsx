import { useInfiniteQuery, useMutation, useQuery, useQueryClient, type InfiniteData } from "@tanstack/react-query";
import type { MessageDto, CreateMessageRequest, PaginatedItems, UpdateMessageRequest } from "../api/dtos";
import { getMessages, createMessage, updateMessage, deleteMessage } from "../api/messages/messagesApi";


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

    return useMutation<MessageDto, Error, { channelId: string; data: CreateMessageRequest }>({
        mutationFn: ({ channelId, data }) => createMessage(channelId, data),
        onSuccess: (newMessage, variables) => {
            const { channelId } = variables;

            const currentData = queryClient.getQueryData<InfiniteData<PaginatedItems<MessageDto>>>(['messages', newMessage.channelId]);
            const messageExists = currentData?.pages.some(page => page.items.some(msg => msg.id === newMessage.id));
            if (messageExists)
                return;

            queryClient.setQueryData<InfiniteData<PaginatedItems<MessageDto>>>(
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
            queryClient.setQueryData<MessageDto[]>(
                ["messages", channelId],
                (oldMessages = []) =>
                    oldMessages.filter((msg) => msg.id !== variables.messageId)
            );
        },
    });
};