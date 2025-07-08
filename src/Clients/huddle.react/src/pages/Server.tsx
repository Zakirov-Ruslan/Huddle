import { useQuery } from "@tanstack/react-query";
import { useState } from "react";

function Server() {

    const [activeChannel, setActiveChannel] = useState('general');
    const [message, setMessage] = useState('');
    const [messages, setMessages] = useState([
        { id: 1, user: 'Alice', text: 'Hello everyone!', timestamp: '10:00 AM' },
        { id: 2, user: 'Bob', text: 'Hi Alice!', timestamp: '10:05 AM' },
    ]);

    const channels = ['general', 'announcements', 'rules', 'web-dev', 'mobile-app', 'design', 'random', 'memes', 'games'];
    const [isSidebarOpen, setIsSidebarOpen] = useState(false);

    const serverId = '06b6b8c5-86e8-42af-9933-a29c0ef8c93a';

    const { isPending, error, data } = useQuery({
        queryKey: ['repoData'],
        queryFn: () =>
            fetch(`https://localhost:7062/channel/api/servers/${serverId}`).then((res) =>
                res.json(),
            ),
    })

    const handleSendMessage = (e) => {
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

        <div className="flex h-screen flex-col bg-gray-800 text-white md:flex-row">
            {/* Mobile Sidebar Toggle */}
            <div className="md:hidden">
                <button
                    className="m-4 rounded bg-gray-700 p-2"
                    onClick={() => setIsSidebarOpen(!isSidebarOpen)}
                >
                    <svg className="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path
                            strokeLinecap="round"
                            strokeLinejoin="round"
                            strokeWidth="2"
                            d="M4 6h16M4 12h16M4 18h16"
                        />
                    </svg>
                </button>
            </div>

            {/* Sidebar */}
            <aside
                className={`fixed inset-y-0 left-0 z-10 flex w-64 flex-col bg-gray-100 dark:bg-gray-900 transition-transform duration-300 ease-in-out md:static md:translate-x-0 ${isSidebarOpen ? 'translate-x-0' : '-translate-x-full'
                    }`}
            >
                <div className="flex h-14 items-center justify-center border-b border-gray-700 text-lg font-semibold">
                    Server
                </div>
                <nav className="flex-1 overflow-y-auto p-4">
                    <ul className="space-y-2">
                        {channels.map((channel) => (
                            <li key={channel}>
                                <button
                                    onClick={() => {
                                        setActiveChannel(channel);
                                        setIsSidebarOpen(false);
                                    }}
                                    className={`w-full text-left rounded-md px-3 py-2 transition-colors ${activeChannel === channel
                                            ? 'bg-blue-600 text-white'
                                            : 'hover:bg-gray-700'
                                        }`}
                                >
                                    # {channel}
                                </button>
                            </li>
                        ))}
                    </ul>
                </nav>
            </aside>

            {/* Overlay for mobile */}
            {isSidebarOpen && (
                <div
                    className="bg-opacity-50 fixed inset-0 z-0 bg-black md:hidden"
                    onClick={() => setIsSidebarOpen(false)}
                ></div>
            )}

            {/* Main Chat Area */}
            <main className="flex flex-1 flex-col md:flex-1">
                {/* Channel Header */}
                <header className="flex h-14 items-center justify-between border-b border-gray-200 bg-white px-4 shadow-sm dark:border-gray-700 dark:bg-gray-800">
                    <h2 className="font-semibold text-gray-800 dark:text-slate-200">#{activeChannel}</h2>
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

                {/* Messages */}
                <section className="flex-1 space-y-4 overflow-y-auto bg-gray-50 p-4 dark:bg-gray-900">
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

                {/* Message Input */}
                <form onSubmit={handleSendMessage} className="border-t border-gray-200 bg-white p-4 dark:border-gray-700 dark:bg-gray-800">
                    <div className="flex items-center space-x-2">
                        <input
                            type="text"
                            value={message}
                            onChange={(e) => setMessage(e.target.value)}
                            placeholder={`Message #${activeChannel}`}
                            className="flex-1 rounded-md border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 px-4 py-2 outline-none focus:ring-2 focus:ring-blue-500 dark:text-slate-200"
                        />
                        <button
                            type="submit"
                            className="rounded-md bg-blue-600 px-4 py-2 text-white transition-colors hover:bg-blue-700"
                        >
                            Send
                        </button>
                    </div>
                </form>
            </main>
        </div>
        
    );
}

export default Server;