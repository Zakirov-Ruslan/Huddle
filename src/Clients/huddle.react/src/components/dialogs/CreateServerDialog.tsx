import { useState } from "react";
import { FaPlus } from "react-icons/fa6";
import { RxCross2 } from "react-icons/rx";
import type { ServerDto } from "../../api/types";
import { useCreateServer } from "../../hooks/queries/servers";

interface CreateServerDialogProps {
    onCreateServer: (createdServer: ServerDto | null) => void;
}

const CreateServerDialog: React.FC<CreateServerDialogProps> = ({ onCreateServer }) => {

    const createServer = useCreateServer();
    const [serverName, setServerName] = useState('');

    const handleCreate = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        if (createServer.isPending || createServer.isSuccess)
            return;

        createServer.mutate(
            { name: serverName },
            {
                onSuccess: (data, variables, context) => {
                    onCreateServer(data);
                }
            }
        )
    } 

    return (
        <form
            className="w-80 space-y-4 px-2 pb-2 md:space-y-3"
            onSubmit={handleCreate}
        >
            <div className="items-left flex flex-col">
                <div className="flex flex-row items-center">
                    <h3 className="flex-grow text-2xl font-bold text-gray-700">Create new server</h3>
                    <button type="button" onClick={() => onCreateServer(null)}>
                        <RxCross2 />
                    </button>
                </div>
            </div>
            <div className="text-center">
                <span>Personalize your server by choosing a name and image. You can change them later.</span>
            </div>
            <div className="flex flex-row items-center justify-center select-none">
                <div className="flex h-25 w-25 flex-row items-center justify-center rounded-3xl border-1 border-dashed">
                    <div className="flex h-20 w-20 cursor-pointer flex-col items-center justify-center rounded-2xl bg-gray-600 text-white transition-transform duration-100 hover:scale-105">
                        <FaPlus className="h-8 w-8" />
                        <span className="text-sm font-medium">Upload</span>
                    </div>
                </div>
            </div>

            <div>
                <label className="mb-2 block text-sm font-medium text-gray-900 dark:text-white">Server name</label>
                <input
                    onChange={(event: React.ChangeEvent<HTMLInputElement>) => setServerName(event.target.value)}
                    autoFocus={true}
                    type="text"
                    name="project-name"
                    className="p-2.5 block w-full rounded-lg border border-gray-300 bg-gray-50 text-sm text-gray-900 focus:ring-primary-600 focus:border-primary-600 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                    required
                    placeholder="new-server"
                />
            </div>
            <div className="flex w-full flex-row justify-end">
                <button
                    type="submit"
                    className="mt-1 w-25 rounded-md bg-indigo-500 p-2 font-medium text-white transition-colors duration-150 hover:bg-indigo-600"
                    disabled={createServer.isPending }
                    
                >
                    {createServer.isPending ? 'Creating...': 'Create'}
                </button>
            </div>

        </form>
    );
    
}

export default CreateServerDialog;