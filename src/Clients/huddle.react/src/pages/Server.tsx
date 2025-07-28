import { useState } from "react";
import { useServer } from "../api/servers/serverApiHooks";
import { RiAttachment2 } from "react-icons/ri";
import { IoSend } from "react-icons/io5";
import { adjustHeight } from "../utils/domHelpers";
import { FaHashtag } from "react-icons/fa6";

function Server() {

    const [activeChannel, setActiveChannel] = useState('general');
    const [message, setMessage] = useState('');
    const [messages, setMessages] = useState([
        { id: 1, user: 'Alice', text: 'Hello everyone!', timestamp: '10:00 AM' },
        { id: 2, user: 'Bob', text: 'Hi Alice!', timestamp: '10:05 AM' },
    ]);

    const channels = ['general', 'announcements', 'rules', 'web-dev', 'mobile-app', 'design', 'random', 'memes', 'games'];

    const serverId = '06b6b8c5-86e8-42af-9933-a29c0ef8c93a';

    const { data: server, error, isPending } = useServer(serverId);

    const handleSendMessage = (e:any) => {
        e.preventDefault();
        if (!message.trim()) return;

        const newMessage = {
            id: messages.length + 1,
            user: 'You',
            text: message,
            timestamp: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
        };

        setMessages([...messages, newMessage]);
        setMessage('');
    };
    return (
        <>
            <div className="flex items-center justify-center border-b-1 border-gray-300 text-lg font-semibold">
                Server
            </div>
            <header className="flex items-center justify-between border-b-1 border-gray-300 bg-white px-4 dark:border-gray-700 dark:bg-gray-800">
                <h2 className="flex flex-row items-center gap-1 font-semibold text-gray-800 dark:text-slate-200">
                    <FaHashtag />
                    {activeChannel}
                </h2>
                <div className="flex space-x-2">
                    <button className="rounded p-1 hover:bg-gray-200 dark:hover:bg-gray-700">
                        <svg className="h-5 w-5 text-gray-600 dark:text-slate-200" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
                        </svg>
                    </button>
                    <button className="rounded p-1 hover:bg-gray-200 dark:hover:bg-gray-700">
                        <svg className="h-5 w-5 text-gray-600 dark:text-slate-200" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z" />
                        </svg>
                    </button>
                </div>
            </header>
            <nav className="flex-1 overflow-y-auto p-4">
                <ul className="space-y-1">
                    {channels.map((channel) => (
                        <li key={channel}>
                            <button
                                type="button"
                                onClick={() => {
                                    setActiveChannel(channel);
                                }}
                                className={`w-full flex gap-2 flex-row items-center text-left rounded-xl px-3 py-1.5 transition-colors ${activeChannel === channel
                                    ? 'bg-gray-300 text-white'
                                    : 'hover:bg-gray-200'
                                    }`}
                            >
                                <FaHashtag/>
                                {channel}
                            </button>
                        </li>
                    ))}
                </ul>
            </nav>
            <div className="flex flex-col">
                <section className="flex-1 space-y-4 overflow-y-auto bg-white p-4 dark:bg-gray-900">
                    {messages.map((msg) => (
                        <div key={msg.id} className="flex items-start space-x-2">
                            <div className="flex h-8 w-8 items-center justify-center rounded-full bg-blue-500 text-sm font-medium text-white">
                                {msg.user.charAt(0)}
                            </div>
                            <div className="flex-1">
                                <div className="flex items-center space-x-2">
                                    <span className="font-medium text-gray-800 dark:text-slate-200">{msg.user}</span>
                                    <span className="text-xs text-gray-500 dark:text-gray-400">{msg.timestamp}</span>
                                </div>
                                <p className="mt-1 text-gray-700 dark:text-slate-200">{msg.text}</p>
                            </div>
                        </div>
                    ))}
                </section>
                <main className="bg-white p-5">
                    <form onSubmit={handleSendMessage}
                        className="flex flex-grow flex-row items-center gap-2 rounded-xl border border-gray-200 bg-white p-2 shadow-2xl dark:border-gray-600 dark:bg-gray-800">
                            <RiAttachment2 className="scale-150"/>
                        <textarea
                            onInput={adjustHeight}
                            rows={1}
                                value={message}
                                onChange={(e) => setMessage(e.target.value)}
                                placeholder={`Message #${activeChannel}`}
                            className=" flex-1  dark:bg-gray-700 px-4 py-2 outline-none resize-none"
                            />
                            <button
                                type="submit"
                                className="w-8"
                            >
                                <IoSend className="scale-150"/>
                            </button>
                        </form>
                </main>
            </div>
            
        </>
    );
}

export default Server;