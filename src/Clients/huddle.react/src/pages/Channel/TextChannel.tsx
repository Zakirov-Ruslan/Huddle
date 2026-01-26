import { useEffect, useMemo, useRef, useState } from "react";
import { IoSend } from "react-icons/io5";
import { RiAttachment2 } from "react-icons/ri";
import { useInView } from "react-intersection-observer";
import { useAuth } from "react-oidc-context";
import type { ChannelDto, MessageDto } from "../../api/dtos";
import AuthorMessageGroup from "../../components/AuthorMessageGroup";
import { useInfiniteMessages, useSendMessage } from "../../hooks/messagesApiHooks";
import { SignalRContext } from "../../providers/SignalRProvider";
import { adjustHeight } from "../../utils/domHelpers";
import { groupMessagesByDayAndAuthor } from "../../utils/groupMessages";
import "../../styles/scrollbar.css";

const TextChannel: React.FC<ChannelDto> = ({ id, serverId, name, channelType }) => {

    const auth = useAuth();

    const {
        data: messages,
        isFetching,
        isFetchingNextPage,
        hasNextPage,
        fetchNextPage,
    } = useInfiniteMessages(id);

    const sendMessage = useSendMessage();

    const [message, setMessage] = useState('');

    const listRef = useRef<HTMLDivElement>(null);
    const textareaRef = useRef<HTMLTextAreaElement>(null);

    const [loaderRef, inView] = useInView();

    const groupedMessages = useMemo(() => {
        if (!messages?.pages) return [];

        const allMessages = messages.pages.flatMap(page => page.items);
        const sortedMessages = [...allMessages].sort(
            (a, b) => new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime()
        );

        return groupMessagesByDayAndAuthor(sortedMessages);
    }, [messages]);

    useEffect(() => {
        if (!id || SignalRContext.connection?.state != 'Connected')
            return;
        SignalRContext.invoke("JoinChannel", id);
        console.log('joined to', id);
    }, [id, SignalRContext.connection?.state])

    SignalRContext.useSignalREffect(
        "CreateMessage",
        (newMessage: MessageDto) => {

            if (newMessage.authorId == auth.user?.profile.sub)
                return;

            if (newMessage.channelId === id) {
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
        if (!isFetching && listRef.current && messages?.pages?.[0]?.items?.length) {
            listRef.current?.scrollTo({
                top: listRef.current.scrollHeight,
                behavior: 'instant'
            });
        }
    }, [isFetching, messages?.pages]);

    return (
        <div className="flex flex-grow flex-row">
            <div className="flex flex-grow flex-col">
                <section ref={listRef} className="custom-scrollbar flex flex-1 flex-col gap-4 overflow-y-scroll p-6 dark:bg-gray-900">
                    <div ref={loaderRef} />
                    {isFetchingNextPage && (
                        <div className="loading-indicator">Loading older messages...</div> //TODO:Skeleton loading animation
                    )}
                    {groupedMessages.map((dayGroup, i) => (
                        <div key={i}>
                            <span className="text-sm font-medium">{dayGroup.day}</span>
                            {dayGroup.authorGroups.map((authorGroup, idx) => (
                                <AuthorMessageGroup
                                    key={idx}
                                    authorId={authorGroup.authorId}
                                    messageGroup={authorGroup.messages}
                                />
                            ))}
                        </div>
                    ))}
                </section>
                <main className="bg-white px-5 pb-4">
                    <form
                        onSubmit={(e) => {
                            e.preventDefault();

                            if (message.trim().length == 0)
                                return;

                            sendMessage.mutate(
                                { channelId: id, data: { text: message } },
                                {
                                    onSuccess: () => {
                                        setMessage('');
                                        if (textareaRef.current) {
                                            requestAnimationFrame(() => {
                                                adjustHeight(textareaRef.current!);
                                            });
                                        }
                                    }
                                }
                            )
                        }}
                        className="flex min-h-17 flex-grow flex-row items-center gap-2 rounded-xl border border-gray-200 bg-white p-3 shadow-2xl dark:border-gray-600 dark:bg-gray-800">
                        <button
                            type="button"
                            className="flex h-8 w-8 items-center justify-center"
                        >
                            <RiAttachment2 className="scale-150" />
                        </button>
                        <textarea
                            ref={textareaRef}
                            onInput={(e) => adjustHeight(e.currentTarget)}
                            rows={1}
                            value={message}
                            onChange={(e) => setMessage(e.target.value)}
                            placeholder={`Write to #${name}`}
                            className=" flex-1  dark:bg-gray-700 px-4 py-2 outline-none resize-none"

                            onKeyDown={(e) => {
                                if (e.key === 'Enter' && !e.shiftKey) {
                                    e.preventDefault();
                                    e.currentTarget.form?.requestSubmit();
                                }
                            }}
                        />
                        <button
                            type="submit"
                            className="flex h-8 w-8 items-center justify-center"
                        >
                            <IoSend className="scale-120" />
                        </button>
                    </form>
                </main>
            </div>
        </div>
    );

}

export default TextChannel;