import { useState } from "react";
import { IoSend } from "react-icons/io5";
import { RiAttachment2 } from "react-icons/ri";
import type { ChannelDto } from "../api/dtos";
import { useChannelMessages, useSendMessage } from "../api/messages/messagesApiHooks";
import { adjustHeight } from "../utils/domHelpers";


const TextChannel: React.FC<ChannelDto> = ({ id, serverId, name, channelType }) => {

    const { data: messages, error, isPending } = useChannelMessages(id);
    const sendMessage = useSendMessage();

    const [message, setMessage] = useState('');

    return (
        <div className="flex flex-grow flex-row">
            <div className="flex flex-grow flex-col">
                <section className="flex flex-1 flex-col gap-4 bg-white p-4 dark:bg-gray-900">
                    {messages?.map((msg) => (
                        <div key={msg.id} className="m-t-auto flex items-start space-x-2 first:mt-auto">
                            <div className="flex h-8 w-8 items-center justify-center rounded-full bg-blue-500 text-sm font-medium text-white">
                                {'U'}
                            </div>
                            <div className="flex-1">
                                <div className="flex items-center space-x-2">
                                    <span className="font-medium text-gray-800 dark:text-slate-200">{msg.authorId}</span>
                                    <span className="text-xs text-gray-500 dark:text-gray-400">{msg.sentAt}</span>
                                </div>
                                <p className="mt-1 text-left text-gray-700 dark:text-slate-200">{msg.text}</p>
                            </div>
                        </div>
                    ))}
                </section>
                <main className="bg-white p-5">
                    <form 
                        onSubmit={(e) => {
                            e.preventDefault();
                            sendMessage.mutate(
                                { channelId: id, data: { text: message } },
                                { onSuccess: () => { setMessage('') } }
                            )
                        }}

                        className="flex flex-grow flex-row items-center gap-2 rounded-xl border border-gray-200 bg-white p-2 shadow-2xl dark:border-gray-600 dark:bg-gray-800">
                        <RiAttachment2 className="scale-150" />
                        <textarea
                            onInput={adjustHeight}
                            rows={1}
                            value={message}
                            onChange={(e) => setMessage(e.target.value)}
                            placeholder={`Write to #${name}`}
                            className=" flex-1  dark:bg-gray-700 px-4 py-2 outline-none resize-none"
                        />
                        <button
                            type="submit"
                            className="w-8"
                        >
                            <IoSend className="scale-150" />
                        </button>
                    </form>
                </main>
            </div>
        </div>
    );
    
}

export default TextChannel;