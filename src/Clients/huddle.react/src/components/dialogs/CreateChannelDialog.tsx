import { useState } from "react";
import type { Channel } from "../../api/types";
import { useCreateChannel } from "../../hooks/queries/channels";
import { FaHashtag } from "react-icons/fa6";
import { HiSpeakerWave } from "react-icons/hi2";
import { RxCross2 } from "react-icons/rx";

interface CreateChannelDialogProps {
    onCreateChannel: (createdChannel: Channel | null) => void;
    serverId: string;
    initialChannelType: 'text' | 'voice'
}

const CreateChannelDialog: React.FC<CreateChannelDialogProps> = ({ onCreateChannel, serverId, initialChannelType }) => {

    const [channelName, setChannelName] = useState("");
    const [channelType, setChannelType] = useState(initialChannelType);

    const createChannel = useCreateChannel(serverId);

    const handleNameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setChannelName(e.target.value);
    };

    const handleTypeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setChannelType(e.target.value);
    };

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        if (createChannel.isPending || createChannel.isSuccess)
            return;

        createChannel.mutate(
            { name: channelName, channelType: channelType },
            {
                onSuccess: (data, variables, context) => {
                    onCreateChannel(data);
                }
            });
    };

    return (
            <div className="w-90 max-w-md bg-white px-2 pb-2">
                <div className="mb-2 flex flex-row items-center text-left">
                    <h3 className="mb-2 flex-grow text-2xl font-bold text-gray-700">Create channel</h3>
                    <button type="button" onClick={() => onCreateChannel(null)}>
                        <RxCross2 />
                    </button>
                </div>

                <form onSubmit={handleSubmit} className="space-y-2">
                    {/* Channel Name Input */}
                    <div>
                        <label htmlFor="channelName" className="mb-2 block text-sm font-medium text-gray-700">
                            Channel name
                        </label>
                        <input
                            type="text"
                            id="channelName"
                            value={channelName}
                            onChange={handleNameChange}
                            placeholder="new-channel"
className="block w-full rounded-lg border border-gray-300 bg-gray-50 p-2.5 text-sm text-gray-900 focus:ring-primary-600 focus:border-primary-600 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-[#5D6D7B] dark:focus:ring-[#5D6D7B]"
                            required
                        />
                    </div>

                    {/* Channel Type Radio Group */}
                    <div>
                        <label className="mb-3 block text-sm font-medium text-gray-700">
                            Channel type
                        </label>
                        <div className="space-y-3">
                        <label className={`flex gap-2 text-gray-700 items-center p-3 border rounded-lg cursor-pointer transition-all duration-200 hover:bg-gray-50 ${channelType === 'text'
                                    ? 'border-[#5D6D7B] bg-gray-100 ring-1 ring-[#5D6D7B]'
                                    : 'border-gray-300 hover:border-gray-400'
                                }`}>
                                <input
                                    type="radio"
                                    name="channelType"
                                    value="text"
                                    checked={channelType === 'text'}
                                    onChange={handleTypeChange}
                                    className="sr-only"
                                    required
                                />
                                <div className={`w-5 h-5 rounded-full border-2 flex items-center justify-center ${channelType === 'text'
                                        ? 'border-[#5D6D7B] bg-[#5D6D7B]'
                                        : 'border-gray-400'
                                    }`}>
                                    {channelType === 'text' && (
                                        <div className="h-2.5 w-2.5 rounded-full bg-white"></div>
                                    )}
                                </div>
                            <FaHashtag />
                            <div className="flex flex-col items-start">
                                <span className="font-medium text-gray-700">Text</span>
                                <span className="text-sm text-gray-700">Send messages, images, gifs or emojis</span>
                            </div>
                            </label>

                            <label className={`flex gap-2 text-gray-700 items-center p-3 border rounded-lg cursor-pointer transition-all duration-200 hover:bg-gray-50 ${channelType === 'voice'
                                    ? 'border-[#5D6D7B] bg-gray-100 ring-1 ring-[#5D6D7B]'
                                    : 'border-gray-300 hover:border-gray-400'
                                }`}>
                                <input
                                    type="radio"
                                    name="channelType"
                                    value="voice"
                                    checked={channelType === 'voice'}
                                    onChange={handleTypeChange}
                                    className="sr-only"
                                />
                                <div className={`w-5 h-5 rounded-full border-2 flex items-center justify-center ${channelType === 'voice'
                                        ? 'border-[#5D6D7B] bg-[#5D6D7B]'
                                        : 'border-gray-400'
                                    }`}>
                                    {channelType === 'voice' && (
                                        <div className="h-2.5 w-2.5 rounded-full bg-white"></div>
                                    )}
                                </div>
                                <HiSpeakerWave />
                                <div className="flex flex-col items-start">
                                    <span className="font-medium text-gray-700">Voice</span>
                                    <span className="text-sm text-gray-700">Communication by voice or video chat</span>
                                </div>
                                
                            </label>
                        </div>
                    </div>

                <div className="flex w-full flex-row items-center justify-end">
                    <button
                        type="submit"
className="mt-1 w-25 rounded-md bg-[#5D6D7B] p-2 font-medium text-white transition-colors duration-150 hover:bg-[#4F5E6A]"
                        disabled={createChannel.isPending }
                    >
                        { createChannel.isPending? 'Creating...' : 'Create'}
                    </button>
                </div>
            </form>
        </div>
    );
}

export default CreateChannelDialog;