import { Link } from "react-router";
import { Menu, MenuButton, MenuItem, MenuItems } from "@headlessui/react";
import { FaHashtag } from "react-icons/fa6";
import { HiSpeakerWave } from "react-icons/hi2";
import { GoPlus } from "react-icons/go";
import { BsPersonFillAdd } from "react-icons/bs";
import { FaPlusCircle } from "react-icons/fa";
import { RiArrowDownSLine } from "react-icons/ri";
import { MdClose } from "react-icons/md";
import { IoIosSettings } from "react-icons/io";
import { ImExit } from "react-icons/im";
import type { Channel, Server, ServerVoiceState } from "../../api/types";
import { AppTooltip } from "../AppTooltip";
import UserPanel from "../layout/UserPanel";
import ServerAvatar from "./ServerAvatar";
import VoiceParticipants from "./VoiceParticipants";
import { useServerVoiceState } from "../../hooks/queries/livekit";
import { Skeleton } from "../skeleton/Skeleton";

export type ChannelType = "text" | "voice";

interface ServerSidebarProps {
    server?: Server;
    serverId: string;
    isPending: boolean;
    isOwner: boolean;
    activeChannelId?: string;
    className?: string;
    onCreateChannel: (channelType: ChannelType) => void;
    onInviteFriends: () => void;
}

interface ChannelListProps {
    title: string;
    channelType: ChannelType;
    channels: Channel[];
    activeChannelId?: string;
    isPending: boolean;
    canCreate: boolean;
    skeletonCount: number;
    onCreate: () => void;
    /** Voice presence of the server, used to show who sits in each voice channel. */
    voiceState?: ServerVoiceState;
}

const ChannelList = ({
    title,
    channelType,
    channels,
    activeChannelId,
    isPending,
    canCreate,
    skeletonCount,
    onCreate,
    voiceState,
}: ChannelListProps) => {
    const Icon = channelType === "text" ? FaHashtag : HiSpeakerWave;
    const tooltipId = `create-${channelType}-channel-tooltip`;

    return (
        <div className="mb-3">
            <div className="mb-2 flex flex-row items-center px-3 text-left text-sm font-semibold text-gray-500">
                <span className="flex-grow">{title}</span>
                {canCreate && (
                    <>
                        <button
                            type="button"
                            onClick={onCreate}
                            data-tooltip-id={tooltipId}
                            aria-label={`Create ${channelType} channel`}
                            className="rounded p-0.5 transition-colors duration-150 hover:bg-gray-200"
                        >
                            <GoPlus />
                        </button>
                        <AppTooltip id={tooltipId} place="top">Create channel</AppTooltip>
                    </>
                )}
            </div>

            <ul className="space-y-1">
                {isPending
                    ? Array.from({ length: skeletonCount }).map((_, i) => (
                        <li key={i}>
                            <Skeleton className="h-8 w-full" />
                        </li>
                    ))
                    : channels.map(channel => (
                        <li key={channel.id}>
                            <Link
                                to={`ch/${channel.id}`}
                                className={`flex w-full flex-row items-center gap-2 rounded-xl px-3 py-1.5 text-left transition-colors ${
                                    channel.id === activeChannelId ? "bg-gray-300" : "hover:bg-gray-200"
                                }`}
                            >
                                <Icon className="shrink-0" />
                                <span className="truncate">{channel.name}</span>
                            </Link>
                            {channelType === "voice" && (
                                <VoiceParticipants
                                    participants={voiceState?.channels.find(state => state.channelId === channel.id)?.participants ?? []}
                                />
                            )}
                        </li>
                    ))}
            </ul>
        </div>
    );
};

/**
 * Second column: server menu + text/voice channel lists.
 *
 * On phones it becomes the whole screen while no channel is selected,
 * from `md` up it is a fixed-width column next to the chat.
 */
function ServerSidebar({
    server,
    serverId,
    isPending,
    isOwner,
    activeChannelId,
    className = "",
    onCreateChannel,
    onInviteFriends,
}: ServerSidebarProps) {
    const textChannels = server?.channels.filter(ch => ch.channelType === "Text") ?? [];
    const voiceChannels = server?.channels.filter(ch => ch.channelType === "Voice") ?? [];

    const { data: voiceState } = useServerVoiceState(serverId);

    return (
        <aside
            className={`flex min-h-0 w-full shrink-0 flex-col border-gray-300 bg-gray-100 md:w-60 md:border-r lg:w-[300px] ${className}`}
        >
            <Menu>
                <MenuButton
                    type="button"
                    className="flex h-[50px] w-full shrink-0 items-center border-b border-gray-300 px-3 text-lg font-semibold text-gray-700 transition-colors duration-150 hover:bg-gray-200 focus:not-data-focus:outline-none md:px-5"
                >
                    {({ active }) => (
                        <>
                            {server == null ? (
                                <Skeleton shape="text" className="mr-2 h-6 w-32 flex-grow" />
                            ) : (
                                <>
                                    {server.avatarUrl && (
                                        <ServerAvatar
                                            name={server.name}
                                            avatarUrl={server.avatarUrl}
                                            className="mr-2 hidden h-6 w-6 items-center justify-center rounded-full bg-[#5D6D7B] text-xs text-white"
                                            imageClassName="mr-2 h-6 w-6 rounded-full object-cover"
                                        />
                                    )}
                                    <span className="mr-2 flex-grow truncate text-left">{server.name}</span>
                                </>
                            )}
                            <span className="shrink-0">{active ? <MdClose /> : <RiArrowDownSLine />}</span>
                        </>
                    )}
                </MenuButton>
                <MenuItems
                    anchor="bottom"
                    transition
                    className="flex w-56 origin-top flex-col rounded-xl border-1 border-gray-200 bg-white p-2 shadow-md transition duration-200 ease-out focus:outline-none [--anchor-gap:--spacing(1)] data-closed:scale-95 data-closed:opacity-0"
                >
                    {isOwner ? (
                        <>
                            <MenuItem>
                                <Link
                                    to={`/server-settings/${serverId}`}
                                    className="flex flex-row items-center rounded p-2 text-left text-sm font-semibold text-gray-700 hover:bg-gray-100"
                                >
                                    <span className="flex-grow">Server settings</span>
                                    <IoIosSettings />
                                </Link>
                            </MenuItem>
                            <MenuItem>
                                <button
                                    type="button"
                                    className="flex flex-row items-center rounded p-2 text-left text-sm font-semibold text-gray-700 hover:bg-gray-100"
                                    onClick={onInviteFriends}
                                >
                                    <span className="flex-grow">Invite friends</span>
                                    <BsPersonFillAdd />
                                </button>
                            </MenuItem>
                            <MenuItem>
                                <button
                                    type="button"
                                    className="flex flex-row items-center rounded p-2 text-left text-sm font-semibold text-gray-700 hover:bg-gray-100"
                                    onClick={() => onCreateChannel("text")}
                                >
                                    <span className="flex-grow">Create channel</span>
                                    <FaPlusCircle />
                                </button>
                            </MenuItem>
                        </>
                    ) : (
                        <MenuItem>
                            <button
                                type="button"
                                className="flex flex-row items-center rounded p-2 text-left text-sm font-semibold text-red-500 hover:bg-gray-100"
                                onClick={() => { }}
                            >
                                <span className="flex-grow">Leave channel</span>
                                <ImExit />
                            </button>
                        </MenuItem>
                    )}
                </MenuItems>
            </Menu>

            <nav className="min-h-0 flex-1 overflow-y-auto p-3 md:p-4">
                <ChannelList
                    title="Text channels"
                    channelType="text"
                    channels={textChannels}
                    activeChannelId={activeChannelId}
                    isPending={isPending}
                    canCreate={isOwner}
                    skeletonCount={2}
                    onCreate={() => onCreateChannel("text")}
                />
                <ChannelList
                    title="Voice channels"
                    channelType="voice"
                    channels={voiceChannels}
                    activeChannelId={activeChannelId}
                    isPending={isPending}
                    canCreate={isOwner}
                    skeletonCount={3}
                    onCreate={() => onCreateChannel("voice")}
                    voiceState={voiceState}
                />
            </nav>

            {/* User card + voice status on phones/tablets. It is pinned to the viewport so it can
                start over the server rail (`left-4`) exactly like the desktop card, while the spacer
                above keeps the last channel of the list clear of it. On `md`+ the right edge keeps
                the same 16px gap to the channel list: 100% - rail(4rem) - shell border(1px) - list(15rem) + gap(1rem). */}
            <div className="shrink-0 px-2 pt-2 pb-3 md:pb-4 lg:hidden">
                <div className="h-14 md:h-17" />
            </div>

            <div className="pointer-events-none fixed right-4 bottom-3 left-4 z-30 md:right-[calc(100%_-_4rem_-_1px_-_15rem_+_1rem)] md:bottom-4 lg:hidden">
                <UserPanel className="pointer-events-auto" />
            </div>
        </aside>
    );
}

export default ServerSidebar;
