import { Link, Outlet } from "react-router";
import { GoHomeFill } from "react-icons/go";
import { useAuth } from "react-oidc-context";
import { FaMicrophone, FaMicrophoneSlash } from "react-icons/fa";
import { TbHeadphonesOff } from "react-icons/tb";
import { useMyServers } from "../api/servers/serverApiHooks";
import { FaCirclePlus } from "react-icons/fa6";
import ReactModal from 'react-modal';
import CreateServerDialog from "../dialogs/CreateServerDialog";
import { useState } from "react";
import { Navigate } from "react-router";
import { useNavigate } from "react-router";
import type { ServerDto } from "../api/dtos";

function Layout() {

    const navigate = useNavigate();
    const auth = useAuth();

    const { data: servers, error, isPending } = useMyServers();
    const [isModalOpen, setIsModalOpen] = useState<boolean>(false)

    switch (auth.activeNavigator) {
        case "signinSilent":
            return <div>Signing you in...</div>;
        case "signoutRedirect":
            return <div>Signing you out...</div>;
    }

    if (auth.isLoading) {
        return <div>Loading...</div>;
    }

    if (auth.error) {
        return <div>Oops... {auth.error.name} caused {auth.error.message}</div>;
    }

    if (auth.isAuthenticated) {

        return (
            <>
                <div className="grid h-full grid-cols-[auto_1fr] bg-gray-100">
                        <div className="flex flex-col items-center gap-3 p-4">
                            <Link to="/h" className="align-center flex h-15 w-15 items-center justify-center rounded-xl bg-gray-200">
                                <GoHomeFill className="h-8 w-8 text-black" />
                            </Link>
                            <div className="my-1 w-13 border-b border-slate-300"></div>
                        {
                            servers?.map(server =>
                                <Link
                                    key={ server.id}
                                    to={`s/${server.id}`}
                                    className="align-center flex h-15 w-15 items-center justify-center rounded-xl bg-gray-200"
                                >
                                    {server.name} 
                                        {/*Here should be an image or first letter of server*/}
                                </Link>
                            )}
                        <button
                            type="button"
                            title="new-server"
                            onClick={() => setIsModalOpen(true) }
                            className="align-center flex h-15 w-15 items-center justify-center rounded-xl bg-gray-200">
                                <FaCirclePlus className="scale-150" />
                        </button>
                            <div className="absolute bottom-4 left-4 z-10 flex h-17 w-90 flex-row gap-2 rounded-xl border-1 border-gray-200 bg-white p-2 shadow-2xl">
                                <div className="flex-grow-1">
                                    {auth.user?.profile.name ?? 'username must be here'}
                                </div>
                                <button className="">
                                    <FaMicrophone />
                                </button>
                                <button className="">
                                    <TbHeadphonesOff />
                                </button>
                            </div>
                        </div>
                    <div className="flex flex-row pt-4">
                        <div className="grid flex-grow-1 grid-cols-[300px_1fr] grid-rows-[50px_1fr] overflow-hidden rounded-tl-2xl border-1 border-gray-300 bg-gray-100">
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
                    <CreateServerDialog onCreateServer={(createdServer: ServerDto) => { setIsModalOpen(false); navigate(`/s/${createdServer.id}`) } } />
                </ReactModal>
            </>
        );
    }

    auth.signinRedirect();

}

export default Layout;

