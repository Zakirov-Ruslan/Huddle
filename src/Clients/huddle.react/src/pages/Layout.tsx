import { Link, Outlet, useParams } from "react-router";
import { FaMicrophone } from "react-icons/fa";
import { TbHeadphonesOff } from "react-icons/tb";
import { FaCirclePlus } from "react-icons/fa6";
import ReactModal from 'react-modal';
import { useState } from "react";
import { useNavigate } from "react-router";
import { Tooltip } from 'react-tooltip'
import face from "../img/huddle-mascot/face.png";
import type { ServerDto } from "../api/types";
import CreateServerDialog from "../components/dialogs/CreateServerDialog";
import { useMyServers } from "../hooks/queries/servers";
import { useMyProfile } from "../hooks/queries/users";

function Layout() {

    const { serverId: currentServerId } = useParams();
    const navigate = useNavigate();

    const { data: servers, error: serversError, isPending: isServersPending } = useMyServers();
    const [isModalOpen, setIsModalOpen] = useState<boolean>(false);

    const { data: profile, isLoading: isProfileLoading, error: profileError } = useMyProfile();

    return (
        <>
            <div className="relative grid h-full grid-cols-[6rem_1fr] overflow-x-hidden bg-gray-100">
                <div className="no-scrollbar flex flex-col items-center gap-3 overflow-y-auto p-4">
                    <Link
                        to="/h"
                        className="align-center flex h-15 w-15 flex-shrink-0 items-center justify-center rounded-xl bg-gray-200"
                        data-tooltip-id={'nav-home'}
                    >
                        <img src={face} alt="logo" className="h-12 w-12 -scale-x-100" />
                    </Link>
                    <Tooltip
                        id='nav-home'
                        style={{
                            backgroundColor: "rgb(255, 255, 255)", color: "#222", borderRadius: "10px", fontWeight: "500", padding: "5px 10px 8px 10px", boxShadow: "0 20px 25px -5px rgb(0 0 0 / 0.1)"
                        }}
                        opacity={1}
                        border="1px solid #e8e8e8"
                        place="right"
                    >
                        home
                    </Tooltip>

                    <div className="my-1 w-13 border-b border-slate-300"></div>
                    {
                        servers?.map(server =>
                            <div key={server.id}>
                                <Link
                                    to={`s/${server.id}`}
                                    replace
                                    onClick={(e) => {
                                        if (currentServerId === server.id) {
                                            e.preventDefault();
                                        }
                                    }}
                                    className="align-center flex h-15 w-15 flex-shrink-0 items-center justify-center rounded-xl bg-gray-200 text-xl font-medium text-gray-600 hover:text-gray-700"
                                    data-tooltip-id={`server-item-tooltip-${server.id}`}
                                >
                                    {server.name[0]}
                                </Link>
                                <Tooltip
                                    id={`server-item-tooltip-${server.id}`} data-tooltip-content={server.name}
                                    style={{
                                        backgroundColor: "rgb(255, 255, 255)", color: "#222", borderRadius: "10px", fontWeight: "500", padding: "5px 10px 8px 10px", boxShadow: "0 20px 25px -5px rgb(0 0 0 / 0.1)"
                                    }}
                                    opacity={1}
                                    border="1px solid #e8e8e8"
                                    place="right"
                                >

                                    {server.name}
                                </Tooltip>
                            </div>
                        )}
                    <button
                        type="button"
                        title="new-server"
                        onClick={() => setIsModalOpen(true)}
                        className="align-center flex h-15 w-15 items-center justify-center rounded-xl bg-gray-200 flex-shrink-0"
                        data-tooltip-id='nav-new-server'
                    >
                        <FaCirclePlus className="scale-150" />
                    </button>
                    <Tooltip
                        id='nav-new-server'
                        style={{
                            backgroundColor: "rgb(255, 255, 255)", color: "#222", borderRadius: "10px", fontWeight: "500", padding: "5px 10px 8px 10px", boxShadow: "0 20px 25px -5px rgb(0 0 0 / 0.1)"
                        }}
                        opacity={1}
                        border="1px solid #e8e8e8"
                        place="right"
                    >
                        add server
                    </Tooltip>
                    <div className="absolute bottom-4 left-4 flex h-17 w-90 flex-row items-center gap-1 rounded-xl border-1 border-gray-200 bg-white p-3 shadow-2xl select-none">
                        <div className="rounded-l-4xl flex flex-grow-1 items-center gap-2 rounded-r-lg transition-colors duration-150 hover:bg-gray-100">
                            <div className="flex h-10 w-10 items-center justify-center rounded-full bg-[#5D6D7B] text-sm font-medium text-white">
                                {profile?.userName[0]}
                            </div>
                            <span className="font-medium">{profile?.userName}</span>
                        </div>

                        <button className="flex h-8 w-8 items-center justify-center rounded-md transition-colors duration-150 hover:bg-gray-100">
                            <FaMicrophone />
                        </button>
                        <button className="flex h-8 w-8 items-center justify-center rounded-md transition-colors duration-150 hover:bg-gray-100">
                            <TbHeadphonesOff />
                        </button>
                    </div>
                </div>
                <div className="flex flex-row overflow-hidden pt-4">
                    <div className="grid h-full flex-grow-1 grid-cols-[300px_1fr] grid-rows-[50px_1fr] overflow-hidden rounded-tl-2xl border-1 border-gray-300 bg-gray-100">
                        <Outlet />
                    </div>
                </div>
            </div>

            <ReactModal
                isOpen={isModalOpen}
                className="modal"
                overlayClassName="modal-overlay"
                onRequestClose={() => setIsModalOpen(false)}
                closeTimeoutMS={150}
                shouldFocusAfterRender={false}
                appElement={document.getElementById('root')!}
            >
                <CreateServerDialog onCreateServer={(createdServer: ServerDto | null) => {
                    setIsModalOpen(false);
                    if (createdServer)
                        navigate(`/s/${createdServer.id}`)
                }} />
            </ReactModal>
        </>
    );
}


export default Layout;

