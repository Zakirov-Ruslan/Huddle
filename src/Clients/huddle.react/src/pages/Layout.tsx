import { Link, Outlet, useParams } from "react-router";
import { GoHomeFill } from "react-icons/go";
import { useAuth } from "react-oidc-context";
import { FaMicrophone } from "react-icons/fa";
import { TbHeadphonesOff } from "react-icons/tb";
import { useMyServers } from "../hooks/serverApiHooks";
import { FaCirclePlus } from "react-icons/fa6";
import ReactModal from 'react-modal';
import CreateServerDialog from "../dialogs/CreateServerDialog";
import { useState } from "react";
import { useNavigate } from "react-router";
import type { ServerDto } from "../api/dtos";
import { Tooltip } from 'react-tooltip'
import { useMyProfile } from "../hooks/usersApiHooks";
import face from "../img/huddle-mascot/face.png";

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
                        <Link to="/h" className="align-center flex h-15 w-15 flex-shrink-0 items-center justify-center rounded-xl bg-gray-200">
                            <img src={face} alt="logo" className="h-12 w-12 -scale-x-100" />
                            </Link>
                            <div className="my-1 w-13 border-b border-slate-300"></div>
                        {
                            servers?.map(server =>
                                <div key = { server.id }>
                                    <Link
                                        to={`s/${server.id}`}
                                        replace
                                        onClick={(e) => {
                                            if (currentServerId === server.id) {
                                                e.preventDefault();
                                            }
                                        }}
                                        className="align-center flex h-15 w-15 flex-shrink-0 items-center justify-center rounded-xl bg-gray-200 text-xl font-medium text-gray-600 hover:text-gray-700"
                                    >
                                        {server.name[0]}
                                    </Link>
                                    <Tooltip
                                        id={`server-item-tooltip-${server.id}`} data-tooltip-content="Hello to you too!"
                                        style={{
                                            backgroundColor: "rgb(255, 255, 255)", color: "#222", borderRadius: "10px", fontWeight: "500", padding: "5px 10px 8px 10px", boxShadow: "0 20px 25px -5px rgb(0 0 0 / 0.1)" }}
                                        opacity={1}
                                        border= "1px solid #e8e8e8"
                                        place="right"
                                    >
                                        
                                        {server.name}
                                    </Tooltip>
                                </div>
                            )}
                        <button
                            type="button"
                            title="new-server"
                            onClick={() => setIsModalOpen(true) }
                            className="align-center flex h-15 w-15 items-center justify-center rounded-xl bg-gray-200 flex-shrink-0">
                                <FaCirclePlus className="scale-150" />
                        </button>
                        <div className="absolute bottom-4 left-4 flex flex h-17 w-90 flex-row items-center gap-1 rounded-xl border-1 border-gray-200 bg-white p-3 shadow-2xl select-none">
                            <div className="rounded-l-4xl flex flex-grow-1 items-center gap-2 rounded-r-lg transition-colors duration-150 hover:bg-gray-100">
                                <div className="flex h-10 w-10 items-center justify-center rounded-full bg-[#5D6D7B] text-sm font-medium text-white">
                                    {profile?.userName[0]}
                                </div>
                                <span className="font-medium">{ profile?.userName }</span>
                            </div>

                            <button className="flex h-8 w-8 items-center justify-center rounded-md transition-colors duration-150 hover:bg-gray-100">
                                <FaMicrophone />
                            </button>
                            <button className="flex h-8 w-8 items-center justify-center rounded-md transition-colors duration-150 hover:bg-gray-100">
                                <TbHeadphonesOff />
                            </button>
                            </div>
                        </div>
                    <div className="flex flex-row pt-4"> 
                        <div className="grid h-full flex-grow-1 grid-cols-[300px_1fr] grid-rows-[50px_calc(100vh_-_calc(48px_+_1.25rem))] overflow-hidden rounded-tl-2xl border-1 border-gray-300 bg-gray-100">
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

