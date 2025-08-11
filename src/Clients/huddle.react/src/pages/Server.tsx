import { useEffect, useState } from "react";
import { useServer } from "../hooks/serverApiHooks";
import { FaHashtag } from "react-icons/fa6";
import { useNavigate, useParams } from "react-router";
import type { ChannelDto, ServerDto } from "../api/dtos";
import { HiSpeakerWave } from "react-icons/hi2";
import { FaUsers } from "react-icons/fa";
import { createContext, useContext } from "react";
import { Outlet } from "react-router";
import { GoPlus } from "react-icons/go";
import { Tooltip } from "react-tooltip";
import ReactModal from "react-modal";
import CreateChannelDialog from "../dialogs/CreateChannelDialog";
import { Menu, MenuButton, MenuItem, MenuItems } from '@headlessui/react'
import { BsPersonFillAdd } from "react-icons/bs";
import { IoIosSettings } from "react-icons/io";
import { FaPlusCircle } from "react-icons/fa";
import { RiArrowDownSLine } from "react-icons/ri";
import { MdClose } from "react-icons/md";

export interface ServerContextType {
    server: ServerDto | undefined;
    isShowMembers: boolean;
}
export const ServerContext = createContext<ServerContextType | null>(null);
export const useServerContext = () => {
    const context = useContext(ServerContext);
    //if (!context) throw new Error("useServerContext must be used within Server");
    return context;
};

type ChannelType = 'text' | 'voice';

function Server() {

    const { serverId, channelId } = useParams();
    if (!serverId) {
        return <div>Invalid server ID</div>;
    }

    const [isShowMembers, setIsShowMembers] = useState(false);
    const [activeChannel, setActiveChannel] = useState<ChannelDto | null>(null);
    const [isModalOpen, setIsModalOpen] = useState(false)

    const navigate = useNavigate();

    const { data: server, error, isPending } = useServer(serverId);

    useEffect(() => {
        setActiveChannel(null);
    }, [serverId]);

    useEffect(() => {
        if (server && !activeChannel) {
            const textChannel = server.channels.find(ch => ch.channelType.toLowerCase() == "text");

            if (textChannel) {
                setActiveChannel(textChannel);
                navigate(`ch/${textChannel.id}`);
            }
        }
    }, [server, activeChannel, navigate]);

    return (
        <>
            <ServerContext.Provider value={{ server: server, isShowMembers: isShowMembers }}>
                <Menu>
                    <MenuButton type="button" className="justify-left flex items-center border-b border-gray-300 px-5 text-lg font-semibold text-gray-700 transition-colors duration-150 hover:bg-gray-200 focus:not-data-focus:outline-none">
                        {({ active }) => (
                            <>
                                {server == null ? (
                                    <span className="h-6 w-32 animate-pulse rounded-xl bg-gray-200"></span>
                                ) : (
                                    <span className="flex-grow text-left">{server.name}</span>
                                )}
                                {active ? <MdClose /> : <RiArrowDownSLine />}
                            </>
                        )}
                    </MenuButton>
                    <MenuItems
                        anchor="bottom"
                        className="flex w-56 origin-top-right flex-col rounded-md border-1 border-gray-200 bg-white p-2 shadow-md transition duration-100 ease-out focus:outline-none [--anchor-gap:--spacing(1)]"
                    >
                        <MenuItem>
                            <button type="button" className="flex flex-row items-center rounded p-2 text-left text-sm font-semibold text-gray-700 hover:bg-gray-100">
                                <span className="flex-grow">Server settings</span>
                                <IoIosSettings />
                            </button>
                        </MenuItem>
                        <MenuItem>
                            <button type="button" className="flex flex-row items-center rounded p-2 text-left text-sm font-semibold text-gray-700 hover:bg-gray-100">
                                <span className="flex-grow">Invite friends</span>
                                <BsPersonFillAdd />
                            </button>
                        </MenuItem>
                        <MenuItem>
                            <button type="button" className="flex flex-row items-center rounded p-2 text-left text-sm font-semibold text-gray-700 hover:bg-gray-100">
                                <span className="flex-grow">Create channel</span>
                                <FaPlusCircle />
                            </button>
                        </MenuItem>
                    </MenuItems>
                </Menu>
                <header className="flex items-center justify-between border-b-1 border-gray-300 bg-white px-4 dark:border-gray-700 dark:bg-gray-800">
                    <h2 className="flex flex-row items-center gap-1 font-semibold text-gray-700 dark:text-slate-200">
                        <FaHashtag />
                        {activeChannel?.name}
                    </h2>
                    <div className="flex space-x-2">
                        <button className="rounded p-1 hover:bg-gray-200 dark:hover:bg-gray-700">
                            <svg className="h-5 w-5 text-gray-600 dark:text-slate-200" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
                            </svg>
                        </button>
                        <button
                            onClick={() => setIsShowMembers(!isShowMembers)}
                            className="rounded p-1 hover:bg-gray-200 dark:hover:bg-gray-700">
                            <FaUsers />
                        </button>
                    </div>
                </header>
                <nav className="flex-1 overflow-y-auto p-4">
                    <div className="mb-2 flex flex-row px-3 text-left text-sm font-semibold text-gray-500 hover:text-gray-600">
                        <span className="flex-grow">Text channels</span>
                        <button
                            onClick={() => setIsModalOpen(true)}
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
                                        <button
                                            type="button"
                                            onClick={() => {
                                                setActiveChannel(channel);
                                                navigate(`ch/${channel.id}`)
                                            }}
                                            className={`w-full flex gap-2 flex-row items-center text-left rounded-xl px-3 py-1.5 transition-colors ${activeChannel === channel
                                                ? 'bg-gray-300 text-white'
                                                : 'hover:bg-gray-200'
                                                }`}
                                        >
                                            <FaHashtag />
                                            {channel.name}
                                        </button>
                                    </li>))}
                            </>
                        )}
                    </ul>
                    <div className="mb-2 flex flex-row px-3 text-left text-sm font-semibold text-gray-500 hover:text-gray-600">
                        <span className="flex-grow">Voice channels</span>
                        <button
                            onClick={() => setIsModalOpen(true)}
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
                                        <button
                                            type="button"
                                            onClick={() => {
                                                setActiveChannel(channel);
                                                navigate(`ch/${channel.id}`)
                                            }}
                                            className={`w-full flex gap-2 flex-row items-center text-left rounded-xl px-3 py-1.5 transition-colors ${activeChannel === channel
                                                ? 'bg-gray-300 text-white'
                                                : 'hover:bg-gray-200'
                                                }`}
                                        >
                                            <HiSpeakerWave />
                                            {channel.name}
                                        </button>
                                    </li>))}
                            </>
                        )}
                    </ul>
                </nav>
                <Outlet />
            </ServerContext.Provider>

            <ReactModal
                isOpen={isModalOpen}
                className="modal"
                overlayClassName="modal-overlay"
                onRequestClose={() => setIsModalOpen(false)}
                closeTimeoutMS={150}
                shouldFocusAfterRender={false}
                appElement={document.getElementById('root')!}
            >
                <CreateChannelDialog serverId={serverId} onCreateChannel={(createdChannel: ChannelDto) => { setIsModalOpen(false); setActiveChannel(createdChannel); navigate(`ch/${createdChannel.id}`) }} />
            </ReactModal>
        </>
    );
}

export default Server;