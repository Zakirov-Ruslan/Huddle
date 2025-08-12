import { useState } from "react";
import type { ChannelDto } from "../api/dtos";
import { useCreateChannel } from "../hooks/channelApiHooks";

interface CreateServerDialogProps {
    onCreateChannel: (createdChannel: ChannelDto) => void;
    serverId: string;
}

const CreateChannelDialog: React.FC<CreateServerDialogProps> = ({ onCreateChannel, serverId }) => {

    const [channelName, setChannelName] = useState("");
    const [channelType, setChannelType] = useState('');

    const createChannel = useCreateChannel(serverId);

    const handleNameChange = (e) => {
        setChannelName(e.target.value);
    };

    const handleTypeChange = (e) => {
        setChannelType(e.target.value);
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        createChannel.mutate(
            { name: channelName, channelType: channelType },
            {
                onSuccess: (data, variables, context) => {
                    onCreateChannel(data);
                }
            });
    };

    return (
            <div className="w-full max-w-md bg-white p-8">
                <div className="mb-8 text-center">
                    <h1 className="mb-2 text-2xl font-bold text-gray-800">Create channel</h1>
                    {/*<p className="text-sm text-gray-600"></p>*/}
                </div>

                <form onSubmit={handleSubmit} className="space-y-6">
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
                            placeholder=""
                            className="placeholder-gray-400 w-full rounded-lg border border-gray-300 px-4 py-3 text-gray-900 transition-all duration-200 focus:border-transparent focus:ring-2 focus:ring-indigo-500"
                            required
                        />
                    </div>

                    {/* Channel Type Radio Group */}
                    <div>
                        <label className="mb-3 block text-sm font-medium text-gray-700">
                            Channel type
                        </label>
                        <div className="space-y-3">
                            <label className={`flex items-center p-3 border rounded-lg cursor-pointer transition-all duration-200 hover:bg-gray-50 ${channelType === 'text'
                                    ? 'border-indigo-500 bg-indigo-50 ring-1 ring-indigo-500'
                                    : 'border-gray-300 hover:border-gray-400'
                                }`}>
                                <input
                                    type="radio"
                                    name="channelType"
                                    value="text"
                                    checked={channelType === 'text'}
                                    onChange={handleTypeChange}
                                    className="sr-only"
                                />
                                <div className={`w-5 h-5 rounded-full border-2 mr-3 flex items-center justify-center ${channelType === 'text'
                                        ? 'border-indigo-500 bg-indigo-500'
                                        : 'border-gray-400'
                                    }`}>
                                    {channelType === 'text' && (
                                        <div className="h-2.5 w-2.5 rounded-full bg-white"></div>
                                    )}
                                </div>
                                <span className="font-medium text-gray-700">Text</span>
                            </label>

                            <label className={`flex items-center p-3 border rounded-lg cursor-pointer transition-all duration-200 hover:bg-gray-50 ${channelType === 'voice'
                                    ? 'border-indigo-500 bg-indigo-50 ring-1 ring-indigo-500'
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
                                <div className={`w-5 h-5 rounded-full border-2 mr-3 flex items-center justify-center ${channelType === 'voice'
                                        ? 'border-indigo-500 bg-indigo-500'
                                        : 'border-gray-400'
                                    }`}>
                                    {channelType === 'voice' && (
                                        <div className="h-2.5 w-2.5 rounded-full bg-white"></div>
                                    )}
                                </div>
                                <span className="font-medium text-gray-700">Voice</span>
                            </label>
                        </div>
                    </div>

                    {/* Submit Button */}
                    <button
                        type="submit"
                        disabled={!channelName.trim()}
                        className="w-full transform rounded-lg bg-indigo-600 px-4 py-3 font-medium text-white shadow-md transition-all duration-200 hover:scale-[1.02] hover:bg-indigo-700 hover:shadow-lg active:scale-[0.98] disabled:cursor-not-allowed disabled:bg-gray-300"
                    >
                        Create
                    </button>
                </form>
            </div>
    );
}

export default CreateChannelDialog;