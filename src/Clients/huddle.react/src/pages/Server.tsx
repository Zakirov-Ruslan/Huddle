import { useEffect, useMemo, useState } from "react";
import { FaHashtag } from "react-icons/fa6";
import { Link, useNavigate, useParams } from "react-router";
import { HiSpeakerWave } from "react-icons/hi2";
import { FaUsers } from "react-icons/fa";
import { Outlet } from "react-router";
import { GoPlus } from "react-icons/go";
import { Tooltip } from "react-tooltip";
import { Menu, MenuButton, MenuItem, MenuItems } from '@headlessui/react'
import { BsPersonFillAdd } from "react-icons/bs";
import { FaPlusCircle } from "react-icons/fa";
import { RiArrowDownSLine } from "react-icons/ri";
import { MdClose } from "react-icons/md";
import { IoIosSettings } from "react-icons/io";
import { useAuth } from "react-oidc-context";
import { ImExit } from "react-icons/im";
import { SignalRContext, useSignalRState } from "../providers/SignalRProvider";
import ReactModal from "react-modal";
import type { Channel, Server } from "../api/types";
import CreateChannelDialog from "../components/dialogs/CreateChannelDialog";
import InviteFriendsDialog from "../components/dialogs/InviteFriendsDialog";
import { useServer } from "../hooks/queries/servers";
import { useApplicationStore } from '../stores/applicationStore';

type ChannelType = 'text' | 'voice';

function Server() {
    const { serverId, channelId } = useParams();
    if (!serverId)
        return <div>Invalid server ID</div>;

    const setActiveServer = useApplicationStore((state) => state.setActiveServer);

    const { isConnected } = useSignalRState();

    const toggleMembersPanedOpened = useApplicationStore((state) => state.toggleMembersPanedOpened);
    const isMembersPanelOpened = useApplicationStore((state) => state.isMembersPanelOpened);

    const [isCreateChannelModalOpen, setIsCreateChannelModalOpen] = useState(false)
    const [isInviteModalOpen, setIsInviteModalOpen] = useState(false)

    const navigate = useNavigate();

    const { data: server, error, isPending } = useServer(serverId);
    const auth = useAuth();

    const activeChannel = useMemo(() => {
        if (!server || !channelId) return null;
        return server.channels.find(ch => ch.id === channelId) ?? null;
    }, [server, channelId]);

    useEffect(() => {
        if (serverId) {
            setActiveServer(server);
        }
    }, [server, setActiveServer]);

    useEffect(() => {
        if (!serverId || !isConnected)
            return;

        const serverJoin = async () => {
            try {
                await SignalRContext.invoke("JoinServer", serverId);
                console.debug('joined to server notification group', serverId);
            } catch (error) {
                console.error('Failed to join server notification group', error);
            }
        }

        serverJoin();

    }, [serverId, isConnected])

    const isServerOwner = server && auth.user && server.ownerIdentityId == auth.user.profile.sub;

    const handleChannelCreated = (createdChannel: Channel | null) => {
        if (createdChannel == null)
            return;

        setIsCreateChannelModalOpen(false);
        navigate(`ch/${createdChannel.id}`)
    }

    return (
        <>
            <Menu>
                <MenuButton type="button" className="justify-left flex items-center border-b border-gray-300 px-5 text-lg font-semibold text-gray-700 transition-colors duration-150 hover:bg-gray-200 focus:not-data-focus:outline-none">
                    {({ active }) => (
                        <>
                            {server == null ? (
                                <span className="h-6 w-32 flex-grow animate-pulse rounded-xl bg-gray-200"></span>
                            ) : (
                                <span className="flex-grow text-left">{server.name}</span>
                            )}
                            {active ? <MdClose /> : <RiArrowDownSLine />}
                        </>
                    )}
                </MenuButton>
                <MenuItems
                    anchor="bottom"
                    transition
                    className="flex w-56 origin-top flex-col rounded-xl border-1 border-gray-200 bg-white p-2 shadow-md transition duration-200 ease-out focus:outline-none [--anchor-gap:--spacing(1)] data-closed:scale-95 data-closed:opacity-0"
                >
                    {isServerOwner ? (
                        <>
                            <MenuItem>
                                <Link to={`/server-settings/${serverId}`} className="flex flex-row items-center rounded p-2 text-left text-sm font-semibold text-gray-700 hover:bg-gray-100">
                                    <span className="flex-grow">Server settings</span>
                                    <IoIosSettings />
                                </Link>
                            </MenuItem>
                            <MenuItem>
                                <button
                                    type="button"
                                    className="flex flex-row items-center rounded p-2 text-left text-sm font-semibold text-gray-700 hover:bg-gray-100"
                                    onClick={() => setIsInviteModalOpen(true)}
                                >
                                    <span className="flex-grow">Invite friends</span>
                                    <BsPersonFillAdd />
                                </button>
                            </MenuItem>
                            <MenuItem>
                                <button
                                    type="button"
                                    className="flex flex-row items-center rounded p-2 text-left text-sm font-semibold text-gray-700 hover:bg-gray-100"
                                    onClick={() => setIsCreateChannelModalOpen(true)}
                                >
                                    <span className="flex-grow">Create channel</span>
                                    <FaPlusCircle />
                                </button>
                            </MenuItem>
                        </>)
                        : (<MenuItem>
                            <button
                                type="button"
                                className="flex flex-row items-center rounded p-2 text-left text-sm font-semibold text-red-500 hover:bg-gray-100"
                                onClick={() => { }}
                            >
                                <span className="flex-grow">Leave channel</span>
                                <ImExit />
                            </button>
                        </MenuItem>)}
                </MenuItems>
            </Menu>
            <header className="flex items-center justify-between border-b-1 border-gray-300 bg-white px-4 dark:border-gray-700 dark:bg-gray-800">
                <h2 className="flex flex-row items-center gap-1 font-semibold text-gray-700 dark:text-slate-200">
                    <FaHashtag />
                    {activeChannel?.name}
                </h2>
                <div className="flex space-x-2">
                    <button
                        onClick={() => toggleMembersPanedOpened()}
                        className="rounded p-1 hover:bg-gray-200 dark:hover:bg-gray-700"
                        data-tooltip-id='members-panel-switcher'
                    >
                        <FaUsers />
                    </button>
                </div>
                <Tooltip
                    id='members-panel-switcher'
                    style={{
                        backgroundColor: "rgb(255, 255, 255)",
                        color: "#222",
                        borderRadius: "10px",
                        fontWeight: "500",
                        padding: "5px 10px 8px 10px",
                        boxShadow: "0 20px 25px -5px rgb(0 0 0 / 0.1)",
                        zIndex: "1"
                    }}
                    opacity={1}
                    border="1px solid #e8e8e8"
                    place="top-end"
                >
                    {isMembersPanelOpened ? "Show list of members" : "Hide list of members"}
                </Tooltip>
            </header>
            <nav className="flex-1 overflow-y-auto p-4">
                <div className="mb-2 flex flex-row px-3 text-left text-sm font-semibold text-gray-500 hover:text-gray-600">
                    <span className="flex-grow">Text channels</span>
                    {isServerOwner &&
                        <>
                            <button
                                onClick={() => setIsCreateChannelModalOpen(true)}
                                data-tooltip-id='create-text-channel-tooltip'
                                type="button">
                                <GoPlus />
                            </button>

                            <Tooltip
                                id='create-text-channel-tooltip' data-tooltip-content=""
                                style={{ backgroundColor: "rgb(255, 255, 255)", color: "#222", borderRadius: "10px", fontWeight: "500", padding: "5px 10px 8px 10px", boxShadow: "0 20px 25px -5px rgb(0 0 0 / 0.1)" }}
                                opacity={1}
                                border="1px solid #e8e8e8"
                                place="top"
                            >
                                Create channel
                            </Tooltip>
                        </>
                    }
                </div>
                <ul className="mb-3 space-y-1">
                    {isPending ? (
                        <>
                            {[...Array(2)].map((_, i) => (
                                <li key={i} className="animate-pulse">
                                    <div className="h-8 w-full rounded-xl bg-gray-200"></div>
                                </li>
                            ))}
                        </>
                    ) : (
                        <>
                            {server?.channels.filter(ch => ch.channelType == "Text").map((channel) => (
                                <li key={channel.id}>
                                    <Link
                                        to={`ch/${channel.id}`}
                                        className={`w-full flex gap-2 flex-row items-center text-left rounded-xl px-3 py-1.5 transition-colors ${activeChannel === channel
                                            ? 'bg-gray-300' : 'hover:bg-gray-200'}`}
                                    >
                                        <FaHashtag />
                                        {channel.name}
                                    </Link>
                                </li>))}
                        </>
                    )}
                </ul>
                <div className="mb-2 flex flex-row px-3 text-left text-sm font-semibold text-gray-500 hover:text-gray-600">
                    <span className="flex-grow">Voice channels</span>
                    {isServerOwner &&
                        <>
                            <button
                                onClick={() => setIsCreateChannelModalOpen(true)}
                                data-tooltip-id='create-voice-channel-tooltip'
                                type="button">
                                <GoPlus />
                            </button>
                            <Tooltip
                                id='create-voice-channel-tooltip' data-tooltip-content=""
                                style={{ backgroundColor: "rgb(255, 255, 255)", color: "#222", borderRadius: "10px", fontWeight: "500", padding: "5px 10px 8px 10px", boxShadow: "0 20px 25px -5px rgb(0 0 0 / 0.1)" }}
                                opacity={1}
                                border="1px solid #e8e8e8"
                                place="top"
                            >
                                Create channel
                            </Tooltip>
                        </>
                    }
                </div>
                <ul className="space-y-1">

                    {isPending ? (
                        <>
                            {[...Array(3)].map((_, i) => (
                                <li key={i} className="animate-pulse">
                                    <div className="h-8 w-full rounded-xl bg-gray-200"></div>
                                </li>
                            ))}
                        </>
                    ) : (
                        <>
                            {server?.channels.filter(ch => ch.channelType == "Voice").map((channel) => (
                                <li key={channel.id}>
                                    <Link
                                        to={`ch/${channel.id}`}
                                        className={`w-full flex gap-2 flex-row items-center text-left rounded-xl px-3 py-1.5 transition-colors ${activeChannel === channel
                                            ? 'bg-gray-300'
                                            : 'hover:bg-gray-200'
                                            }`}
                                    >
                                        <HiSpeakerWave />
                                        {channel.name}
                                    </Link>
                                </li>))}
                        </>
                    )}
                </ul>
            </nav>
            <Outlet key={channelId} />

            <ReactModal
                isOpen={isCreateChannelModalOpen}
                className="modal"
                overlayClassName="modal-overlay"
                onRequestClose={() => setIsCreateChannelModalOpen(false)}
                closeTimeoutMS={150}
                shouldFocusAfterRender={false}
                appElement={document.getElementById('root')!}
            >
                <CreateChannelDialog
                    serverId={serverId}
                    onCreateChannel={handleChannelCreated}
                    initialChannelType={'text'}
                />
            </ReactModal>

            <ReactModal
                isOpen={isInviteModalOpen}
                className="modal"
                overlayClassName="modal-overlay"
                onRequestClose={() => setIsInviteModalOpen(false)}
                closeTimeoutMS={150}
                shouldFocusAfterRender={false}
                appElement={document.getElementById('root')!}
            >
                <InviteFriendsDialog serverId={serverId} />
            </ReactModal>
        </>
    );
}

export default Server;