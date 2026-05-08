import { useEffect, useMemo, useRef, useState, type FormEvent } from "react";
import { IoSend } from "react-icons/io5";
import { RiAttachment2 } from "react-icons/ri";
import { useInView } from "react-intersection-observer";
import { useAuth } from "react-oidc-context";
import type { Channel, Message } from "../../api/types";
import AuthorMessageGroup from "../../components/AuthorMessageGroup";
import { SignalRContext, useSignalRState } from "../../providers/SignalRProvider";
import { adjustHeight } from "../../utils/domHelpers";
import { groupMessagesByDayAndAuthor } from "../../utils/groupMessages";
import "../../styles/scrollbar.css";
import { useInfiniteMessages, useSendMessage } from "../../hooks/queries/messages";
import { useTextChannelStore, type LocalMessage } from '../../stores/textChannelStore';

const TextChannel: React.FC<Channel> = ({ id, serverId, name, channelType }) => {

    const auth = useAuth();

    const { isConnected } = useSignalRState();

    const draftText = useTextChannelStore((state) => state.drafts[id] || '');
    const setDraft = useTextChannelStore((state) => state.setDraft);
    const localMessages = useTextChannelStore((state) => state.localMessages[id]);

    const {
        data: messages,
        isFetching,
        isFetchingNextPage,
        hasNextPage,
        fetchNextPage,
    } = useInfiniteMessages(id);

    const sendMessage = useSendMessage();

    const listRef = useRef<HTMLDivElement>(null);
    const textareaRef = useRef<HTMLTextAreaElement>(null);

    const [loaderRef, inView] = useInView();

    const allMessages = useMemo(() => {

        const serverMessages = messages?.pages?.flatMap(page => page.items) || [];
        const serverMessagesAsLocal: LocalMessage[] = serverMessages.map(msg => ({
            ...msg,
            status: "success" as const,
        }));

        const combined = localMessages 
            ? [...serverMessagesAsLocal, ...localMessages] 
            : serverMessagesAsLocal;

        return combined.sort((a, b) =>
            new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime()
        );
    }, [messages, localMessages]);

    const groupedMessages = useMemo(() => {
        return groupMessagesByDayAndAuthor(allMessages);
    }, [allMessages]);

    const handleInputChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
        const newText = e.target.value;
        setDraft(id, newText);
        adjustHeight(e.currentTarget);
    };

    useEffect(() => {
        if (!id || !isConnected)
            return;

        const channelJoin = async () => {
            try {
                await SignalRContext.invoke("JoinChannel", id);
                console.debug('joined to channel notification group', id);
            } catch (error) {
                console.error('Failed to join channel notification group', error);
            }
        }

        channelJoin();
    }, [id, isConnected])

    useEffect(() => {
        if (inView && hasNextPage && !isFetchingNextPage) {
            fetchNextPage();
        }
    }, [inView, hasNextPage, fetchNextPage, isFetchingNextPage]);

    useEffect(() => {
        if (!isFetching && listRef.current && allMessages?.length) {
            listRef.current?.scrollTo({
                top: listRef.current.scrollHeight,
                behavior: 'instant'
            });
        }
    }, [isFetching, allMessages]);

    const handleSendMessage = (e: FormEvent<HTMLFormElement>): void => {
        e.preventDefault();

        if (draftText.trim().length == 0)
            return;

        sendMessage.mutate(
            {
                channelId: id,
                data: { text: draftText }
            }
        )

        setDraft(id, '');
        if (textareaRef.current) {
            requestAnimationFrame(() => {
                adjustHeight(textareaRef.current!);
            });
        }
    }

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
                <div className="bg-white px-5 pb-4">
                    <form
                        onSubmit={handleSendMessage}
                        className="flex min-h-17 flex-grow flex-row items-center gap-2 rounded-xl border border-gray-200 bg-white p-3 shadow-2xl dark:border-gray-600 dark:bg-gray-800">
                        <button
                            type="button"
                            className="flex h-8 w-8 items-center justify-center"
                        >
                            <RiAttachment2 className="scale-150" />
                        </button>
                        <textarea
                            ref={textareaRef}
                            rows={1}
                            value={draftText}
                            onChange={handleInputChange}
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
                            className='flex h-8 w-8 items-center justify-center'
                            disabled={!draftText.trim()}
                        >
                            <IoSend className="scale-120" />
                        </button>
                    </form>
                </div>
            </div>
        </div>
    );

}

export default TextChannel;