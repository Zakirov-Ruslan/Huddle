import { useEffect, useRef, useState } from "react";
import { IoSend } from "react-icons/io5";
import { RiAttachment2 } from "react-icons/ri";
import type { ChannelDto, MessageDto, PaginatedItems } from "../api/dtos";
import { useInfiniteMessages, useSendMessage } from "../hooks/messagesApiHooks";
import { adjustHeight } from "../utils/domHelpers";
import { useInView } from 'react-intersection-observer';
import React from "react";
import "../styles/scrollbar.css";
import { SignalRContext } from "../pages/Root";
import { useQueryClient, type InfiniteData } from "@tanstack/react-query";
import { useAuth } from "react-oidc-context";

const TextChannel: React.FC<ChannelDto> = ({ id, serverId, name, channelType }) => {

    const {
        data,
        isFetching,
        isFetchingNextPage,
        hasNextPage,
        fetchNextPage,
    } = useInfiniteMessages(id);

    const sendMessage = useSendMessage();

    const [message, setMessage] = useState('');

    const listRef = useRef<HTMLDivElement>(null);
    const [loaderRef, inView] = useInView();

    useEffect(() => {
        if (!id || SignalRContext.connection?.state != 'Connected')
            return;
        SignalRContext.invoke("JoinChannel", id);
        console.log('joined to', id);
    }, [id, SignalRContext.connection?.state])

    const queryClient = useQueryClient();
    const auth = useAuth();
    SignalRContext.useSignalREffect(
        "CreateMessage",
        (newMessage: MessageDto) => {
            console.log('message received', newMessage); // Правильное логирование объекта
            if (newMessage.authorId == auth.user?.profile.sub)
                return;
            // Проверяем, что сообщение относится к текущему каналу
            if (newMessage.channelId === id) {
                queryClient.setQueryData<InfiniteData<PaginatedItems<MessageDto>>>(
                    ['messages', id],
                    (oldData) => {
                        if (!oldData) {
                            return {
                                pages: [{
                                    items: [newMessage],
                                    hasMore: true,
                                    nextCursor: null
                                }],
                                pageParams: [null]
                            };
                        }

                        // Обновляем первую страницу, добавляя новое сообщение в конец
                        const updatedPages = [...oldData.pages];
                        if (updatedPages.length > 0) {
                            updatedPages[0] = {
                                ...updatedPages[0],
                                items: [...updatedPages[0].items, newMessage] // Добавляем в конец для новых сообщений
                            };
                        }

                        return {
                            ...oldData,
                            pages: updatedPages
                        };
                    }
                );

                // Прокручиваем к новому сообщению
                setTimeout(() => {
                    listRef.current?.scrollTo({
                        top: listRef.current.scrollHeight,
                        behavior: 'smooth'
                    });
                }, 100);
            }
        },
        [id]
    );

    useEffect(() => {
        if (inView && hasNextPage && !isFetchingNextPage) {
            fetchNextPage();
        }
    }, [inView, hasNextPage, fetchNextPage, isFetchingNextPage]);

    useEffect(() => {
        if (!isFetching && listRef.current && data?.pages?.[0]?.items?.length) {
            listRef.current?.scrollTo({
                top: listRef.current.scrollHeight,
                behavior: 'instant'
            });
        }
    }, [isFetching, data?.pages]);

    return (
        <div className="flex flex-grow flex-row">
            <div className="flex flex-grow flex-col">
                <section ref={listRef} className="custom-scrollbar flex flex-1 flex-col gap-4 overflow-y-scroll bg-white p-6 dark:bg-gray-900">
                    <div ref={loaderRef} />
                    {isFetchingNextPage && (
                        <div className="loading-indicator">Loading older messages...</div>
                    )}
                    {data?.pages.flatMap(page => page.items)
                        .sort((a, b) => new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime())
                        .map((msg) => (
                            <div key={msg.id} className="m-t-auto flex items-start space-x-2 first:mt-auto">
                                <div className="flex h-8 w-8 items-center justify-center rounded-full bg-blue-500 text-sm font-medium text-white">
                                    {'U'}
                                </div>
                                <div className="flex-1">
                                    <div className="flex items-center space-x-2">
                                        <span className="font-medium text-gray-800 dark:text-slate-200">{msg.authorId}</span>
                                        <span className="text-xs text-gray-500 dark:text-gray-400">{msg.sentAt.toString()}</span>
                                    </div>
                                    <p className="mt-1 text-left text-gray-700 dark:text-slate-200">{msg.text}</p>
                                </div>
                            </div>

                        ))}

                </section>
                <main className="bg-white px-5 pb-5">
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