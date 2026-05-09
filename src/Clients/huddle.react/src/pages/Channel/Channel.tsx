import { useEffect } from "react";
import { useInView } from "react-intersection-observer";
import ServerMemberItem from "../../components/ServerMemberItem";
import TextChannel from "./TextChannel";
import VoiceChannel from "./VoiceChannel";
import { useParams } from 'react-router';
import { useInfiniteMembers } from "../../hooks/queries/members";
import { useApplicationStore } from '../../stores/applicationStore';
import idunno from "../../img/huddle-mascot/idunno.png";

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
        return (
            <div className="flex flex-col items-center justify-center bg-white select-none">
                <div className="relative max-w-md rotate-1 transform rounded-3xl bg-white p-8 shadow">
                    <div className="text-center">
                        <h2 className="mb-4 text-3xl font-bold tracking-wide text-gray-800">Nothing is here</h2>
                        <p>It looks weird</p>
                    </div>
                </div>


                <img
                    src={idunno}
                    alt="empty content"
                    className="h-60 w-60"
                />
            </div>);

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