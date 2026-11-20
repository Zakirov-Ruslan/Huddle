import { useState } from "react";
import { NavLink, Outlet, useNavigate, useParams } from "react-router";
import { Dialog, DialogPanel } from "@headlessui/react";
import ReactModal from "react-modal";
import { FaArrowLeft, FaBars, FaServer, FaTrash, FaXmark } from "react-icons/fa6";
import { IoIosSettings } from "react-icons/io";
import { useServer } from "../../hooks/queries/servers";
import type { Server } from "../../api/types";
import DeleteServerDialog from "../../components/dialogs/DeleteServerDialog";
import ServerAvatar from "../../components/server/ServerAvatar";
import { Skeleton } from "../../components/skeleton/Skeleton";
import { SettingsSkeleton } from "../../components/skeleton/Skeletons";
import { AppTooltip } from "../../components/AppTooltip";

/** Round icon button shared by the back arrow and the phone burger. */
const roundButtonClasses =
    "flex h-9 w-9 shrink-0 items-center justify-center rounded-full border border-gray-300 bg-white text-gray-600 shadow-sm transition-colors duration-150 hover:bg-gray-200 hover:text-gray-700 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-gray-400 md:h-10 md:w-10";

interface SettingsNavProps {
    serverId: string;
    server?: Server;
    /** Called when a link is followed, so the phone drawer can close itself. */
    onNavigate?: () => void;
    onDeleteServer: () => void;
}

/**
 * Navigation panel of the settings screen: the server it belongs to on top,
 * the sections below and the destructive action pinned to the bottom.
 */
const SettingsNav = ({ serverId, server, onNavigate, onDeleteServer }: SettingsNavProps) => (
    <>
        <div className="flex items-center gap-3 border-b border-gray-200 p-3">
            <div className="flex h-11 w-11 shrink-0 items-center justify-center overflow-hidden rounded-xl bg-[#5D6D7B] text-lg font-semibold text-white">
                {server == null ? (
                    <Skeleton tone="inverse" className="h-full w-full" />
                ) : (
                    <ServerAvatar
                        name={server.name}
                        avatarUrl={server.avatarUrl}
                        className="flex h-full w-full items-center justify-center"
                        imageClassName="h-full w-full object-cover"
                    />
                )}
            </div>
            <div className="min-w-0 flex-1">
                {server == null ? (
                    <Skeleton shape="text" className="h-4 w-28" />
                ) : (
                    <p className="truncate align-start text-sm font-semibold text-gray-800">{server.name}</p>
                )}
                <p className="mt-1 flex items-center gap-1.5 text-xs text-gray-500">
                    <IoIosSettings className="shrink-0" />
                    Server settings
                </p>
            </div>
        </div>

        <nav aria-label="Server settings" className="flex min-h-0 flex-1 flex-col gap-1 overflow-y-auto p-2">
            <span className="px-3 pt-1 pb-1.5 text-xs font-semibold tracking-wide text-gray-400 uppercase">
                Manage
            </span>
            <NavLink
                to={`/server-settings/${serverId}/server-profile`}
                onClick={onNavigate}
                className={({ isActive }) =>
                    `flex items-center gap-3 rounded-xl px-3 py-2 text-sm font-medium transition-colors duration-150 ${
                        isActive ? "bg-gray-200 text-gray-800" : "text-gray-600 hover:bg-gray-100 hover:text-gray-700"
                    }`
                }
            >
                <FaServer className="shrink-0" />
                <span className="truncate">Server profile</span>
            </NavLink>
        </nav>

        <div className="border-t border-gray-200 p-2">
            <button
                type="button"
                onClick={onDeleteServer}
                className="flex w-full items-center gap-3 rounded-xl px-3 py-2 text-sm font-medium text-red-500 transition-colors duration-150 hover:bg-red-50"
            >
                <FaTrash className="shrink-0" />
                <span className="truncate">Delete server</span>
            </button>
        </div>
    </>
);

/**
 * Server settings shell.
 *
 *   [ back ][ navigation card ] [ content card ]
 *
 * From `md` up the navigation is a column on the left; on phones it moves into a
 * drawer behind the burger in the top bar, so the form keeps the whole screen.
 */
function ServerSettings() {

    const { serverId } = useParams();

    const { data: server, error, isPending } = useServer(serverId);
    const navigate = useNavigate(); 

    const [isDeleteServerModalIsOpen, setIsDeleteServerModalIsOpen] = useState<boolean>(false);
    const [isNavOpen, setIsNavOpen] = useState<boolean>(false);

    const handleServerDelete = (isSuccess: boolean) => {
        setIsDeleteServerModalIsOpen(false);
        if (isSuccess) {
            navigate('/h');
        }
    };

    /**
     * Settings are reachable from any channel, so a real history entry restores the exact
     * screen the user came from. Direct links have nothing behind them — go to the server.
     */
    const handleBack = () => {
        if (window.history.state?.idx > 0) {
            navigate(-1);
            return;
        }

        navigate(serverId ? `/s/${serverId}` : '/h', { replace: true });
    };

    const openDeleteServerModal = () => {
        setIsNavOpen(false);
        setIsDeleteServerModalIsOpen(true);
    };

    if (!serverId)
        return (
            <div className="flex h-full w-full items-center justify-center bg-gray-100">
                <p className="text-gray-700">Invalid server ID</p>
            </div>
        );

    if (error) {
        return (
            <div className="flex h-full w-full items-center justify-center bg-gray-100">
                <div className="text-center text-red-600">
                    <p>Error getting server data</p>
                </div>
            </div>
        );
    }

    return (
        <>
            <div className="flex h-full w-full flex-col overflow-hidden bg-gray-100 md:flex-row md:gap-4 md:p-4">
                <header className="flex h-14 shrink-0 items-center gap-2 border-b border-gray-300 px-2 md:hidden">
                    <button
                        type="button"
                        onClick={handleBack}
                        aria-label="Back to server"
                        data-tooltip-id="settings-back-mobile"
                        className={roundButtonClasses}
                    >
                        <FaArrowLeft />
                    </button>
                    <AppTooltip id="settings-back-mobile" place="bottom">Back to server</AppTooltip>

                    <div className="min-w-0 flex-1">
                        {server == null ? (
                            <Skeleton shape="text" className="h-4 w-32" />
                        ) : (
                            <p className="truncate text-sm font-semibold text-gray-700">{server.name}</p>
                        )}
                    </div>

                    <button
                        type="button"
                        onClick={() => setIsNavOpen(true)}
                        aria-label="Open settings navigation"
                        data-tooltip-id="settings-nav-open"
                        className={roundButtonClasses}
                    >
                        <FaBars />
                    </button>
                    <AppTooltip id="settings-nav-open" place="bottom-end">Settings menu</AppTooltip>
                </header>

                <aside className="hidden w-72 shrink-0 flex-col gap-3 md:flex">
                    <button
                        type="button"
                        onClick={handleBack}
                        aria-label="Back to server"
                        data-tooltip-id="settings-back"
                        className={roundButtonClasses}
                    >
                        <FaArrowLeft />
                    </button>
                    <AppTooltip id="settings-back" place="right">Back to server</AppTooltip>

                    <div className="flex min-h-0 flex-1 flex-col overflow-hidden rounded-2xl border border-gray-200 bg-white shadow-sm">
                        <SettingsNav
                            serverId={serverId}
                            server={server}
                            onDeleteServer={openDeleteServerModal}
                        />
                    </div>
                </aside>

                <main className="min-h-0 min-w-0 flex-1 md:overflow-hidden">
                    <div className="h-full overflow-y-auto bg-white md:rounded-2xl md:border md:border-gray-200 md:shadow-sm">
                        {isPending ? <SettingsSkeleton /> : <Outlet />}
                    </div>
                </main>

                <Dialog open={isNavOpen} onClose={() => setIsNavOpen(false)} className="relative z-50 md:hidden">
                    <div
                        aria-hidden="true"
                        className="fixed inset-0 bg-gray-900/30 transition-opacity duration-200 ease-out data-closed:opacity-0"
                    />
                    <div className="fixed inset-0 flex">
                        <DialogPanel
                            transition
                            className="flex w-72 max-w-[85vw] flex-col gap-3 border-r border-gray-300 bg-gray-100 p-3 shadow-xl transition duration-200 ease-out data-closed:-translate-x-full"
                        >
                            <div className="flex items-center justify-between pl-1">
                                <span className="text-sm font-semibold text-gray-600">Server settings</span>
                                <button
                                    type="button"
                                    onClick={() => setIsNavOpen(false)}
                                    aria-label="Close settings navigation"
                                    className={roundButtonClasses}
                                >
                                    <FaXmark />
                                </button>
                            </div>

                            <div className="flex min-h-0 flex-1 flex-col overflow-hidden rounded-2xl border border-gray-200 bg-white shadow-sm">
                                <SettingsNav
                                    serverId={serverId}
                                    server={server}
                                    onNavigate={() => setIsNavOpen(false)}
                                    onDeleteServer={openDeleteServerModal}
                                />
                            </div>
                        </DialogPanel>
                    </div>
                </Dialog>
            </div>

            <ReactModal
                isOpen={isDeleteServerModalIsOpen}
                className="modal"
                overlayClassName="modal-overlay"
                onRequestClose={() => setIsDeleteServerModalIsOpen(false)}
                closeTimeoutMS={150}
                shouldFocusAfterRender={false}
                appElement={document.getElementById('root')!}
            >
                <DeleteServerDialog 
                    serverId={serverId} 
                    onServerDelete={handleServerDelete} 
                />
            </ReactModal>
        </>
    );
}

export default ServerSettings;