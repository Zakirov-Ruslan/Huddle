import { FiX } from "react-icons/fi";
import { HiOutlineDocumentText } from "react-icons/hi2";
import type { PendingAttachment } from "../../hooks/useAttachmentUploads";
import { formatBytes } from "../../utils/fileHelpers";

interface AttachmentPreviewListProps {
    attachments: PendingAttachment[];
    onRemove: (localId: string) => void;
}

const AttachmentPreviewList = ({ attachments, onRemove }: AttachmentPreviewListProps) => {
    if (attachments.length === 0)
        return null;

    return (
        <div className="flex flex-wrap items-end gap-2 px-1 pb-2">
            {attachments.map(attachment => (
                <div
                    key={attachment.localId}
                    className="group relative overflow-hidden rounded-xl border border-gray-200 bg-gray-50 shadow-sm"
                >
                    {attachment.previewUrl ? (
                        <div className="h-20 w-20 md:h-24 md:w-24">
                            <img
                                src={attachment.previewUrl}
                                alt={attachment.file.name}
                                className="h-full w-full object-cover"
                            />
                        </div>
                    ) : (
                        <div className="flex h-20 w-44 items-center gap-2 px-3 md:h-24 md:w-52">
                            <HiOutlineDocumentText className="h-7 w-7 shrink-0 text-[#5D6D7B]" />
                            <div className="flex min-w-0 flex-col">
                                <span className="truncate text-sm font-medium text-gray-700">
                                    {attachment.file.name}
                                </span>
                                <span className="text-xs text-gray-500">
                                    {formatBytes(attachment.file.size)}
                                </span>
                            </div>
                        </div>
                    )}

                    {attachment.status === "uploading" && (
                        <div className="absolute inset-0 flex flex-col items-center justify-center gap-1 bg-black/45">
                            <span className="text-xs font-semibold text-white">{attachment.progress}%</span>
                            <div className="h-1 w-3/4 overflow-hidden rounded-full bg-white/30">
                                <div
                                    className="h-full rounded-full bg-white transition-[width] duration-150"
                                    style={{ width: `${attachment.progress}%` }}
                                />
                            </div>
                        </div>
                    )}

                    {attachment.status === "error" && (
                        <div
                            className="absolute inset-0 flex items-center justify-center bg-red-500/75 px-2 text-center"
                            title={attachment.error ?? "Upload failed"}
                        >
                            <span className="text-xs font-semibold text-white">Upload failed</span>
                        </div>
                    )}

                    <button
                        type="button"
                        onClick={() => onRemove(attachment.localId)}
                        aria-label={`Remove ${attachment.file.name}`}
                        className="absolute right-1 top-1 flex h-5 w-5 items-center justify-center rounded-full bg-black/55 text-white opacity-0 transition-opacity duration-150 hover:bg-black/75 group-hover:opacity-100 focus:opacity-100"
                    >
                        <FiX className="h-3.5 w-3.5" />
                    </button>
                </div>
            ))}
        </div>
    );
};

export default AttachmentPreviewList;
