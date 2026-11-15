import { useEffect, useMemo, useRef, type FormEvent } from "react";
import { IoSend } from "react-icons/io5";
import { RiAttachment2 } from "react-icons/ri";
import { useInView } from "react-intersection-observer";
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
import { useTextChannelStore, type LocalMessage } from '../../stores/textChannelStore';
import { ALLOWED_ATTACHMENT_TYPES, MAX_ATTACHMENTS } from "../../utils/fileHelpers";

const TextChannel: React.FC<Channel> = ({ id, name }) => {
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
    const attachments = useAttachmentUploads("attachment");

    const listRef = useRef<HTMLDivElement>(null);
    const textareaRef = useRef<HTMLTextAreaElement>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    const [loaderRef, inView] = useInView();

    const { isDragging, dropHandlers } = useFileDrop({ onFiles: attachments.addFiles });

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

            <section ref={listRef} className="custom-scrollbar flex min-h-0 flex-1 flex-col gap-4 overflow-y-auto p-4 md:p-6">
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
