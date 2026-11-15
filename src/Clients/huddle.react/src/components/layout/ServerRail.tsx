import { useState } from "react";
import { Link, useNavigate, useParams } from "react-router";
import ReactModal from "react-modal";
import { FaCirclePlus } from "react-icons/fa6";
import face from "../../img/huddle-mascot/face.png";
import type { Server } from "../../api/types";
import CreateServerDialog from "../dialogs/CreateServerDialog";
import { AppTooltip } from "../AppTooltip";
import { useMyServers } from "../../hooks/queries/servers";
import ServerAvatar from "../server/ServerAvatar";

/** Shared look of a single rail button. Compact on phones, roomy from `lg`. */
const railItemClasses =
    "flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-gray-200 text-base font-medium text-gray-600 transition-colors duration-150 hover:text-gray-700 lg:h-15 lg:w-15 lg:text-xl";

/**
 * Leftmost navigation rail: home, server list and "add server".
 * Stays visible on every breakpoint.
 *
 * The user card with the voice connection lives in `UserPanel` (rendered by the
 * layout, not by the rail), so this component stays pure navigation.
 */
function ServerRail() {
    const { serverId: currentServerId } = useParams();
    const navigate = useNavigate();

    const { data: servers } = useMyServers();

    const [isCreateServerModalOpen, setIsCreateServerModalOpen] = useState(false);

    const handleServerCreated = (createdServer: Server | null) => {
        setIsCreateServerModalOpen(false);
        if (createdServer)
            navigate(`/s/${createdServer.id}`);
    };

    return (
        <>
            <aside className="flex w-16 shrink-0 flex-col bg-gray-100 lg:w-24">
                {/* No top padding on phones (icons sit flush, like in a native app);
                    from `md` it mirrors the `pt-4` of the content area. */}
                <div className="no-scrollbar flex min-h-0 flex-1 flex-col items-center gap-2 overflow-y-auto px-1 md:gap-3 md:pt-4 lg:p-4">
                    <Link
                        to="/h"
                        className={`${railItemClasses} overflow-hidden`}
                        data-tooltip-id="nav-home"
                        aria-label="Home"
                    >
                        <img src={face} alt="home" className="h-9 w-9 -scale-x-100 lg:h-12 lg:w-12" />
                    </Link>
                    <AppTooltip id="nav-home" place="right">home</AppTooltip>

                    {servers?.map(server => (
                        <div key={server.id} className="shrink-0">
                            <Link
                                to={`s/${server.id}`}
                                replace
                                onClick={(e) => {
                                    if (currentServerId === server.id) {
                                        e.preventDefault();
                                    }
                                }}
                                className={`${railItemClasses} group overflow-hidden ${currentServerId === server.id ? "bg-gray-300" : ""}`}
                                data-tooltip-id={`server-item-tooltip-${server.id}`}
                            >
                                <ServerAvatar
                                    name={server.name}
                                    avatarUrl={server.avatarUrl}
                                    className="flex h-full w-full items-center justify-center transition-transform duration-200 ease-out group-hover:scale-105"
                                    imageClassName="h-full w-full rounded-xl object-cover transition-transform duration-200 ease-out group-hover:scale-105"
                                />
                            </Link>
                            <AppTooltip id={`server-item-tooltip-${server.id}`} place="right">
                                {server.name}
                            </AppTooltip>
                        </div>
                    ))}

                    <button
                        type="button"
                        onClick={() => setIsCreateServerModalOpen(true)}
                        className={railItemClasses}
                        data-tooltip-id="nav-new-server"
                        aria-label="Create server"
                    >
                        <FaCirclePlus className="scale-125 lg:scale-150" />
                    </button>
                    <AppTooltip id="nav-new-server" place="right">add server</AppTooltip>
                </div>
            </aside>

            <ReactModal
                isOpen={isCreateServerModalOpen}
                className="modal"
                overlayClassName="modal-overlay"
                onRequestClose={() => setIsCreateServerModalOpen(false)}
                closeTimeoutMS={150}
                shouldFocusAfterRender={false}
                appElement={document.getElementById('root')!}
            >
                <CreateServerDialog onCreateServer={handleServerCreated} />
            </ReactModal>
        </>
    );
}

export default ServerRail;
