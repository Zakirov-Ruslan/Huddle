import { useEffect } from "react";
import { useInView } from "react-intersection-observer";
import ServerMemberItem from "../../components/ServerMemberItem";
import TextChannel from "./TextChannel";
import VoiceChannel from "./VoiceChannel";
import { useParams } from 'react-router';
import { useInfiniteMembers } from "../../hooks/queries/members";
import { useApplicationStore } from '../../stores/applicationStore';
import Mascot from "../../components/Mascot";
import { ChatSkeleton, MemberListSkeleton } from "../../components/skeleton/Skeletons";

export const Channel = () => {
    const { channelId } = useParams<{ channelId: string }>();

    const isMembersPanelOpened = useApplicationStore((state) => state.isMembersPanelOpened);
    const setMembersPanelOpened = useApplicationStore((state) => state.setMembersPanelOpened);
    const activeServer = useApplicationStore((state) => state.activeServer);

    const serverId = activeServer?.id;

    const {
        data,
        isPending: areMembersPending,
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
        return <ChatSkeleton />;

    const channel = activeServer.channels.find(ch => ch.id === channelId);
    if (!channel)
        return (
            <div className="flex flex-1 flex-col items-center justify-center gap-2 bg-white select-none">
                <div className="relative max-w-md rotate-1 transform rounded-3xl bg-white p-8 shadow">
                    <div className="text-center">
                        <h2 className="mb-4 text-3xl font-bold tracking-wide text-gray-800">Nothing is here</h2>
                        <p>It looks weird</p>
                    </div>
                </div>

                <Mascot pose="confused" alt="Confused Huddle mascot" className="h-60" />
            </div>);

    const isTextChannel = channel.channelType.toLowerCase() === "text";

    return (
        <div className="relative flex min-h-0 flex-1 flex-row overflow-hidden bg-white" >

            {isTextChannel ? (
                <TextChannel {...channel} />
            ) : channel.channelType.toLowerCase() === "voice" ? (
                <VoiceChannel {...channel} />
            ) : (
                <div>Unsupported channel type</div>
            )}

            {isMembersPanelOpened && isTextChannel && (
                <>
                    {/* Backdrop: on phones the members list is an overlay, not a column. */}
                    <button
                        type="button"
                        aria-label="Close members list"
                        onClick={() => setMembersPanelOpened(false)}
                        className="absolute inset-0 z-20 cursor-default bg-black/20 md:hidden"
                    />

                    <aside className="absolute inset-y-0 right-0 z-30 flex w-72 max-w-[85%] flex-col border-l-1 border-gray-200 bg-white p-1.5 shadow-2xl md:static md:z-auto md:w-60 md:max-w-none md:shrink-0 md:shadow-none">
                        {/* The close button lives in the channel header — no duplicate here. */}
                        <div className="mb-1 px-2 py-1 md:hidden">
                            <span className="text-sm font-semibold text-gray-500">Members</span>
                        </div>

                        <div className="flex min-h-0 flex-1 flex-col gap-1 overflow-y-auto">
                            {areMembersPending && <MemberListSkeleton />}
                            {data?.pages.flatMap(page => page.items).map(member => (
                                <ServerMemberItem key={member.identityId} userId={member.identityId} />
                            ))}
                            <div ref={loaderRef} />
                        </div>
                    </aside>
                </>
            )}
        </div>
    );

};