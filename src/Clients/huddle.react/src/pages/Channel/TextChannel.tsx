import { useCallback, useEffect, useMemo, useRef, type FormEvent } from "react";
import { FiArrowDown } from "react-icons/fi";
import { IoSend } from "react-icons/io5";
import { RiAttachment2 } from "react-icons/ri";
import type { Channel, MessageAttachment } from "../../api/types";
import AuthorMessageGroup from "../../components/AuthorMessageGroup";
import AttachmentPreviewList from "../../components/chat/AttachmentPreviewList";
import FileDropOverlay from "../../components/chat/FileDropOverlay";
import { SignalRContext, useSignalRState } from "../../providers/SignalRProvider";
import { adjustHeight } from "../../utils/domHelpers";
import { groupMessagesByDayAndAuthor } from "../../utils/groupMessages";
import "../../styles/scrollbar.css";
import { useInfiniteMessages, useSendMessage } from "../../hooks/queries/messages";
import { useAttachmentUploads } from "../../hooks/useAttachmentUploads";
import { useFileDrop } from "../../hooks/useFileDrop";
import { useMessageWindow } from "../../hooks/useMessageWindow";
import { useTextChannelStore, type LocalMessage } from '../../stores/textChannelStore';
import { ALLOWED_ATTACHMENT_TYPES, MAX_ATTACHMENTS } from "../../utils/fileHelpers";

const dayKey = (value: Date | string) => new Date(value).toLocaleDateString('ru-RU');

const TextChannel: React.FC<Channel> = ({ id, name }) => {
    const { isConnected } = useSignalRState();

    const draftText = useTextChannelStore((state) => state.drafts[id] || '');
    const setDraft = useTextChannelStore((state) => state.setDraft);
    const localMessages = useTextChannelStore((state) => state.localMessages[id]);

    const {
        data: messages,
        isFetching,
        isFetchingPreviousPage,
        hasPreviousPage,
        fetchPreviousPage,
    } = useInfiniteMessages(id);

    const sendMessage = useSendMessage();
    const attachments = useAttachmentUploads("attachment");

    const textareaRef = useRef<HTMLTextAreaElement>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    const { isDragging, dropHandlers } = useFileDrop({ onFiles: attachments.addFiles });

    const allMessages = useMemo(() => {
        const byId = new Map<string, LocalMessage>();

        for (const page of messages?.pages ?? []) {
            for (const message of page.items) {
                if (!byId.has(message.id))
                    byId.set(message.id, { ...message, status: "success" as const });
            }
        }

        for (const local of localMessages ?? [])
            byId.set(local.id, local);

        return [...byId.values()].sort((a, b) => {
            const byDate = new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime();
            return byDate !== 0 ? byDate : a.id.localeCompare(b.id);
        });
    }, [messages, localMessages]);

    const loadOlder = useCallback(() => {
        void fetchPreviousPage();
    }, [fetchPreviousPage]);

    const {
        containerRef,
        contentRef,
        visibleMessages,
        startIndex,
        isAtBottom,
        hasNewMessages,
        onScroll,
        scrollToBottom,
    } = useMessageWindow({
        channelId: id,
        messages: allMessages,
        hasOlder: Boolean(hasPreviousPage),
        isFetchingOlder: isFetchingPreviousPage,
        onLoadOlder: loadOlder,
    });

    const groupedMessages = useMemo(() => {
        return groupMessagesByDayAndAuthor(visibleMessages);
    }, [visibleMessages]);

    // Do not repeat the day header when the window starts in the middle of a day.
    const hideLeadingDayLabel = useMemo(() => {
        if (startIndex === 0 || visibleMessages.length === 0)
            return false;

        const previous = allMessages[startIndex - 1];

        return previous !== undefined && dayKey(previous.sentAt) === dayKey(visibleMessages[0].sentAt);
    }, [allMessages, startIndex, visibleMessages]);

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

    const resetTextarea = () => {
        if (textareaRef.current) {
            requestAnimationFrame(() => {
                adjustHeight(textareaRef.current!);
            });
        }
    };

    const canSend = (draftText.trim().length > 0 || attachments.attachments.length > 0)
        && !attachments.isUploading
        && !attachments.hasErrors;

    const handleSendMessage = (e: FormEvent<HTMLFormElement>): void => {
        e.preventDefault();

        if (!canSend)
            return;

        const payloadAttachments: MessageAttachment[] = attachments.uploaded.map(file => ({
            fileId: file.id,
            fileName: file.fileName,
            contentType: file.contentType,
            size: file.size,
            isImage: file.isImage,
            url: file.url
        }));

        sendMessage.mutate(
            {
                channelId: id,
                data: { text: draftText, attachmentIds: payloadAttachments.map(file => file.fileId) },
                attachments: payloadAttachments
            }
        )

        setDraft(id, '');
        attachments.clear();
        resetTextarea();
        scrollToBottom(true);
    }

    const handlePaste = (e: React.ClipboardEvent<HTMLTextAreaElement>) => {
        const pasted = Array.from(e.clipboardData.files);
        if (pasted.length === 0)
            return;

        e.preventDefault();
        attachments.addFiles(pasted);
    };

    const handleRemoveAttachment = (localId: string) => {
        attachments.remove(localId);

        if (attachments.attachments.length <= 1)
            attachments.clearRejection();
    };

    return (
        <div
            className="relative flex min-h-0 min-w-0 flex-1 flex-col"
            {...dropHandlers}
        >
            {isDragging && <FileDropOverlay />}

            <section
                ref={containerRef}
                onScroll={onScroll}
                className="custom-scrollbar min-h-0 flex-1 overflow-y-auto p-4 md:p-6"
            >
                <div ref={contentRef} className="flex flex-col gap-4">
                    {isFetchingPreviousPage && (
                        <div className="text-center text-xs font-medium text-gray-400">Loading older messages...</div>
                    )}

                    {allMessages.length === 0 && isFetching && (
                        <div className="text-center text-sm text-gray-400">Loading messages...</div>
                    )}

                    {groupedMessages.map((dayGroup, i) => (
                        <div key={`${dayGroup.day}-${i}`}>
                            {!(i === 0 && hideLeadingDayLabel) && (
                                <span className="text-sm font-medium">{dayGroup.day}</span>
                            )}
                            {dayGroup.authorGroups.map((authorGroup) => (
                                <AuthorMessageGroup
                                    key={authorGroup.messages[0].id}
                                    authorId={authorGroup.authorId}
                                    messageGroup={authorGroup.messages}
                                />
                            ))}
                        </div>
                    ))}
                </div>
            </section>

            {!isAtBottom && (
                <button
                    type="button"
                    onClick={() => scrollToBottom()}
                    aria-label="Jump to the latest messages"
                    title="Jump to the latest messages"
                    className="absolute bottom-24 right-4 z-20 flex h-10 w-10 items-center justify-center rounded-full border border-gray-200 bg-white text-[#5D6D7B] shadow-lg transition-colors duration-150 hover:bg-gray-100 md:right-6"
                >
                    <FiArrowDown className="h-5 w-5" />
                    {hasNewMessages && (
                        <span className="absolute -right-0.5 -top-0.5 h-2.5 w-2.5 rounded-full bg-[#5D6D7B]" />
                    )}
                </button>
            )}

            <div className="shrink-0 bg-white px-3 pb-3 md:px-5 md:pb-4">
                <AttachmentPreviewList
                    attachments={attachments.attachments}
                    onRemove={handleRemoveAttachment}
                />

                {(attachments.rejection || attachments.hasErrors) && (
                    <div className="px-1 pb-2 text-xs font-medium text-red-500">
                        {attachments.rejection ?? "Some files were not uploaded, remove them and try again"}
                    </div>
                )}

                <form
                    onSubmit={handleSendMessage}
                    className="flex min-h-14 flex-grow flex-row items-center gap-2 rounded-xl border border-gray-200 bg-white p-2 shadow-2xl md:min-h-17 md:p-3">
                    <input
                        ref={fileInputRef}
                        type="file"
                        multiple
                        accept={ALLOWED_ATTACHMENT_TYPES.join(",")}
                        className="hidden"
                        onChange={(e) => {
                            const files = Array.from(e.target.files ?? []);
                            e.target.value = "";

                            if (files.length > 0)
                                attachments.addFiles(files);
                        }}
                    />
                    <button
                        type="button"
                        onClick={() => fileInputRef.current?.click()}
                        aria-label="Attach files"
                        title={`Attach files (up to ${MAX_ATTACHMENTS})`}
                        className="flex h-8 w-8 shrink-0 items-center justify-center rounded-md text-gray-600 transition-colors duration-150 hover:bg-gray-100"
                    >
                        <RiAttachment2 className="scale-150" />
                    </button>
                    <textarea
                        ref={textareaRef}
                        rows={1}
                        value={draftText}
                        onChange={handleInputChange}
                        onPaste={handlePaste}
                        placeholder={`Write to #${name}`}
                        className="min-w-0 flex-1 resize-none px-2 py-2 outline-none md:px-4"
                        onKeyDown={(e) => {
                            if (e.key === 'Enter' && !e.shiftKey) {
                                e.preventDefault();
                                e.currentTarget.form?.requestSubmit();
                            }
                        }}
                    />
                    <button
                        type="submit"
                        className='flex h-8 w-8 shrink-0 items-center justify-center disabled:opacity-40'
                        disabled={!canSend}
                        aria-label="Send message"
                    >
                        <IoSend className="scale-120" />
                    </button>
                </form>
            </div>
        </div>
    );

}

export default TextChannel;
