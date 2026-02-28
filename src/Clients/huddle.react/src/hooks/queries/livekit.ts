import { useQuery } from "@tanstack/react-query";
import { getLiveKitToken } from "../../api/liveKit/liveKitApi";

export const useLiveKitToken = (serverId: string, channelId: string) => {
    return useQuery<string, Error>({
        queryKey: ['liveKitToken', channelId],
        queryFn: () => getLiveKitToken(serverId, channelId)
    });
};
