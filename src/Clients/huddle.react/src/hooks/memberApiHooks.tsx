import { useInfiniteQuery } from "@tanstack/react-query";
import { getServerMembers } from "../api/mebmers/membersApi";

export const useInfiniteMembers = (serverId: string) => {
    return useInfiniteQuery({
        queryKey: ['members', serverId],
        queryFn: ({ pageParam }: { pageParam: string | null }) =>
            getServerMembers({
                serverId,
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