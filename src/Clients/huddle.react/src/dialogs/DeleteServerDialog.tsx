import { useDeleteServer, useServer } from "../hooks/serverApiHooks";
import { useState, useEffect } from "react";

interface DeleteServerDialogProps {
    serverId: string,
    onServerDelete: (isSuccess: boolean) => void;
}

const DeleteServerDialog: React.FC<DeleteServerDialogProps> = ({ serverId, onServerDelete }) => {
    const { data: server, error: serverError, isPending: serverIsPending } = useServer(serverId);
    const deleteServer = useDeleteServer();
    
    const [serverNameInput, setServerNameInput] = useState<string>("");
    const [isValidInput, setIsValidInput] = useState<boolean>(false);

    useEffect(() => {
        setIsValidInput(serverNameInput === server?.name);
    }, [serverNameInput, server?.name]);

    const handleDelete = () => {
        if (!isValidInput) return;
        
        deleteServer.mutate(serverId, {
            onSuccess: () => {
                onServerDelete(true);
            },
            onError: (error) => {
                console.error('Error deleting server:', error);
                onServerDelete(false);
            }
        });
    };

    if (serverIsPending) {
        return (
            <div className="p-6">
                <div className="flex items-center justify-center">
                    <div className="h-8 w-8 animate-spin rounded-full border-b-2 border-gray-900"></div>
                </div>
            </div>
        );
    }

    if (serverError) {
        return (
            <div className="p-6">
                <div className="text-center text-red-600">
                    Error getting server data
                </div>
                <div className="mt-4 flex justify-center">
                    <button 
                        type="button" 
                        onClick={() => onServerDelete(false)}
                        className="px-4 py-2 bg-gray-500 text-white rounded hover:bg-gray-600"
                    >
                        Cancel
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className="mx-auto max-w-md p-2">
            <span className="mb-4 text-xl font-bold text-gray-900">
                Delete server
            </span>
            
            <div className="mb-4">
                <p className="mb-2 text-gray-700">
                    Are you sure you want to delete <strong>{server?.name}</strong>? 
                    This action cannot be undone.
                </p>
                <p className="mb-3 text-sm text-gray-600">
                    To confirm, enter the server name:
                </p>
                
                <input
                    type="text"
                    value={serverNameInput}
                    onChange={(e) => setServerNameInput(e.target.value)}
                    placeholder={server?.name}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent"
                />
            </div>

            {deleteServer.error && (
                <div className="mb-4 rounded border border-red-400 bg-red-100 p-3 text-red-700">
                    Error while deleting. Try again.
                </div>
            )}

            <div className="flex justify-end gap-3">
                <button 
                    type="button" 
                    onClick={() => onServerDelete(false)}
                    className="px-4 py-2 bg-gray-500 text-white rounded hover:bg-gray-600 transition-colors"
                    disabled={deleteServer.isPending}
                >
                    Cancel
                </button>
                <button 
                    type="button" 
                    onClick={handleDelete}
                    disabled={!isValidInput || deleteServer.isPending}
                    className={`px-4 py-2 rounded transition-colors ${
                        isValidInput && !deleteServer.isPending
                            ? 'bg-red-600 text-white hover:bg-red-700'
                            : 'bg-gray-300 text-gray-500 cursor-not-allowed'
                    }`}
                >
                    {deleteServer.isPending ? 'Deleting...' : 'Delete server'}
                </button>
            </div>
        </div>
    );
}

export default DeleteServerDialog;