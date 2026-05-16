import type { QueryClient, InfiniteData } from "@tanstack/react-query";
import type { Message, PaginatedItems, SignalRNotification } from "../api/types";

export function handleCreateMessage(queryClient: QueryClient) {
    return (notification: SignalRNotification<Message>) => {
        const newMessage = notification.entity;
        console.debug('message received (global cache handler)', newMessage);

        // const currentData = queryClient.getQueryData<InfiniteData<PaginatedItems<MessageDto>>>(['messages', newMessage.channelId]);
        // const messageExists = currentData?.pages.some(page => page.items.some(msg => msg.id === newMessage.id));
        // if (messageExists)
        //     return;

        queryClient.setQueryData<InfiniteData<PaginatedItems<Message>>>(
            ['messages', newMessage.channelId],
            (oldData) => {
                if (!oldData) {
                    return {
                        pages: [{
                            items: [newMessage],
                            hasPrev: true,
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
    };
}