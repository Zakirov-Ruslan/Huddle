import { useState } from "react";
import type { ChannelDto } from "../api/dtos";

interface CreateServerDialogProps {
    onCreateChannel: (createdChannel: ChannelDto) => void;
}

const CreateChannelDialog: React.FC<CreateServerDialogProps> = ({ onCreateChannel }) => {

    //const createChannel = useCreateChannel();
    const [channelName, setChannelName] = useState('');

    return (
        <form
            className="space-y-4 px-2 pb-2 md:space-y-6"
            onSubmit={(e) => {
                e.preventDefault();

            }}
        >
            <div className="flex flex-col items-center">
                <span className="font-medium">Create new channel</span>
            </div>
            <div>
                <label htmlFor="email" className="mb-2 block text-sm font-medium text-gray-900 dark:text-white">Server name</label>
                <input
                    onChange={(event: React.ChangeEvent<HTMLInputElement>) => setChannelName(event.target.value)}
                    autoFocus={true}
                    type="text"
                    name="project-name"
                    className="p-2.5 block w-full rounded-lg border border-gray-300 bg-gray-50 text-sm text-gray-900 focus:ring-primary-600 focus:border-primary-600 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                    required
                />
            </div>
            <button
                type="submit"
                className="w-full rounded-md bg-blue-700 p-2 font-medium text-white"
            >
                Create
            </button>
        </form>
    );
}

export default CreateChannelDialog;