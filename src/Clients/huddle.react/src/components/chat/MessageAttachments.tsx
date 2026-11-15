import { HiOutlineDocumentText } from "react-icons/hi2";
import type { MessageAttachment } from "../../api/types";
import { resolveFileUrl } from "../../api/files/filesApi";
import { formatBytes } from "../../utils/fileHelpers";

interface MessageAttachmentsProps {
    attachments?: MessageAttachment[];
}

const imageGridClasses = (count: number) => {
    if (count === 1)
        return "w-fit max-w-full";
    if (count === 2)
        return "grid-cols-2 w-full max-w-sm";
    return "grid-cols-2 w-full max-w-sm md:grid-cols-3 md:max-w-md";
};

const MessageAttachments = ({ attachments }: MessageAttachmentsProps) => {
    if (!attachments || attachments.length === 0)
        return null;

    const images = attachments.filter(attachment => attachment.isImage);
    const files = attachments.filter(attachment => !attachment.isImage);

    return (
        <div className="mt-1.5 flex w-full min-w-0 flex-col gap-2">
            {images.length > 0 && (
                <div className={`grid min-w-0 gap-1 ${imageGridClasses(images.length)}`}>
                    {images.map(image => (
                        <a
                            key={image.fileId}
                            href={resolveFileUrl(image.url)}
                            target="_blank"
                            rel="noreferrer"
                            className="group relative block min-w-0 overflow-hidden rounded-lg border border-gray-200 bg-gray-100"
                            title={image.fileName}
                        >
                            <img
                                src={resolveFileUrl(image.url)}
                                alt={image.fileName}
                                loading="lazy"
                                className={`transition-transform duration-150 group-hover:scale-[1.02] ${images.length === 1 ? "max-h-72 w-auto max-w-full object-contain" : "aspect-square w-full object-cover"}`}
                            />
                        </a>
                    ))}
                </div>
            )}

            {files.map(file => (
                <a
                    key={file.fileId}
                    href={`${resolveFileUrl(file.url)}?download=true`}
                    target="_blank"
                    rel="noreferrer"
                    className="flex w-full min-w-0 max-w-sm items-center gap-3 rounded-xl border border-gray-200 bg-gray-50 px-3 py-2 transition-colors duration-150 hover:border-[#5D6D7B] hover:bg-gray-100"
                >
                    <HiOutlineDocumentText className="h-6 w-6 shrink-0 text-[#5D6D7B]" />
                    <span className="flex min-w-0 flex-col">
                        <span className="truncate text-sm font-medium text-gray-700">{file.fileName}</span>
                        <span className="text-xs text-gray-500">{formatBytes(file.size)}</span>
                    </span>
                </a>
            ))}
        </div>
    );
};

export default MessageAttachments;
