import { useEffect } from "react";
import { useInView } from "react-intersection-observer";
import ServerMemberItem from "../../components/ServerMemberItem";
import TextChannel from "./TextChannel";
import VoiceChannel from "./VoiceChannel";
import { useParams } from 'react-router';
import { useInfiniteMembers } from "../../hooks/queries/members";
import { useApplicationStore } from '../../stores/applicationStore'; 

export const Channel = () => {
    const { channelId } = useParams<{ channelId: string }>();

    const isMembersPanelOpened = useApplicationStore((state) => state.isMembersPanelOpened);
    const activeServer = useApplicationStore((state) => state.activeServer);

    const serverId = activeServer?.id; 

    const {
        data,
        isFetching,
        isFetchingNextPage,
        hasNextPage,
        fetchNextPage,
    } = useInfiniteMembers(serverId, Boolean(serverId));
    const [loaderRef, inView] = useInView();

    useEffect(() => {
        if (inView && hasNextPage && !isFetchingNextPage) {
            fetchNextPage();
        }
    }, [inView, hasNextPage, fetchNextPage, isFetchingNextPage]);

    if (!activeServer)
        return <div>Loading...</div>;

    const channel = activeServer.channels.find(ch => ch.id === channelId);
    if (!channel)
        return <div>Nothing is here</div>;;

    return (
        <div className="flex min-h-0 flex-row bg-white" >

            {channel.channelType.toLowerCase() === "text" ? (
                <TextChannel {...channel} />
            ) : channel.channelType.toLowerCase() === "voice" ? (
                <VoiceChannel {...channel} />
            ) : (
                <div>Unsupported channel type</div>
            )}


            {isMembersPanelOpened && channel.channelType.toLowerCase() === "text" && (
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