import TextChannel from "./TextChannel";
import VoiceChannel from "./VoiceChannel";
import { useParams } from 'react-router';
import { useServerContext } from "../pages/Server";
import { useInfiniteMembers } from "../hooks/memberApiHooks";
import { useInView } from "react-intersection-observer";
import { useEffect } from "react";
import ServerMemberItem from "./ServerMemberItem";

export const Channel = () => {
    const { channelId } = useParams<{ channelId: string }>();

    const serverContext = useServerContext();

    const {
        data,
        isFetching,
        isFetchingNextPage,
        hasNextPage,
        fetchNextPage,
    } = useInfiniteMembers(serverContext?.server?.id, Boolean(serverContext && serverContext.server));
    const [loaderRef, inView] = useInView();

    useEffect(() => {
        if (inView && hasNextPage && !isFetchingNextPage) {
            fetchNextPage();
        }
    }, [inView, hasNextPage, fetchNextPage, isFetchingNextPage]);

    if (!serverContext || !serverContext.server)
        return <div>Loading...</div>;

    const channel = serverContext.server.channels.find(ch => ch.id === channelId);
    if (!channel)
        return <div>Nothing is here</div>;;

    return (
        <div className="flex min-h-0 flex-row" >

            {channel.channelType.toLowerCase() === "text" ? (
                <TextChannel {...channel} />
            ) : channel.channelType.toLowerCase() === "voice" ? (
                <VoiceChannel {...channel} />
            ) : (
                <div>Unsupported channel type</div>
            )}


            {serverContext.isShowMembers && channel.channelType.toLowerCase() === "text" && (
                (
                    <div className="flex w-60 flex-col gap-1 border-l-1 border-gray-200 bg-white p-1.5">
                        {data?.pages.flatMap(page => page.items).map(member => (
                            <ServerMemberItem key={member.identityId} userId={member.identityId} />
                        ))}
                        <div ref={loaderRef} />
                    </div>
                )
            )}
        </div>
    );

};