import { useInfiniteQuery } from "@tanstack/react-query";
import { getServerMembers } from "../../api/mebmers/membersApi";

export const useInfiniteMembers = (serverId?: string, enabled: boolean = true) => {
    return useInfiniteQuery({
        queryKey: ['members', serverId ?? 'unknown'],
        queryFn: ({ pageParam }: { pageParam: string | null }) =>
            getServerMembers({
                serverId: serverId as string,
                cursor: pageParam
            }),
        getNextPageParam: (lastPage) => {
            return lastPage.hasMore ? lastPage.nextCursor : undefined;
        },
        initialPageParam: null,
        staleTime: 5 * 60 * 1000, //5 minutes
        refetchOnWindowFocus: false,
        enabled: Boolean(serverId) && enabled,
    });
};