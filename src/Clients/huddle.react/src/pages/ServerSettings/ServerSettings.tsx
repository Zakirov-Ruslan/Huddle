import { useParams, Outlet, Link, useNavigate } from "react-router";
import { FaTrash } from "react-icons/fa";
import ReactModal from "react-modal";
import { useState } from "react";
import { useServer } from "../../hooks/queries/servers";
import DeleteServerDialog from "../../components/dialogs/DeleteServerDialog";

function ServerSettings() {

    const { serverId } = useParams();
    if (!serverId)
        return <div>Invalid server ID</div>;

    const { data: server, error, isPending } = useServer(serverId);
    const navigate = useNavigate(); 

    const [isDeleteServerModalIsOpen, setIsDeleteServerModalIsOpen] = useState<boolean>(false);

    const handleServerDelete = (isSuccess: boolean) => {
        setIsDeleteServerModalIsOpen(false);
        if (isSuccess) {
            navigate('/h');
        }
    };

    if (isPending) {
        return (
            <div className="flex h-full w-full items-center justify-center">
                <div className="h-8 w-8 animate-spin rounded-full border-b-2 border-gray-900"></div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="flex h-full w-full items-center justify-center">
                <div className="text-center text-red-600">
                    <p>Error getting server data</p>
                </div>
            </div>
        );
    }

    return (
        <>
            <div className="flex h-full w-full flex-row bg-gray-200">
                <div className="flex w-1/3 justify-end pt-10">
                    <div className="m-2 flex w-60 flex-col gap-2">
                        <span className="ml-2 px-3 py-1 text-left font-medium">
                            {server?.name}
                        </span>
                        <Link to='server-profile' className="flex items-center rounded-md px-3 py-1 font-medium text-gray-700 transition-colors duration-150 hover:bg-gray-300">
                            Server profile
                        </Link>
                        <button
                            type="button"
                            className="flex items-center rounded-md px-3 py-1 font-medium text-red-400 duration-150 hover:bg-gray-300"
                            onClick={() => setIsDeleteServerModalIsOpen(true)}
                        >
                            <span className="grow text-left">Delete server</span>
                            <FaTrash />
                        </button>
                    </div>
                </div>

                <div className="grow bg-gray-100 pt-10">
                    <Outlet />
                </div>
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