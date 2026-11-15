import { useCallback, useEffect, useRef, useState } from "react";
import { uploadFile, type FileKind, type UploadedFile } from "../api/files/filesApi";
import { MAX_ATTACHMENTS, isImageFile, validateFile } from "../utils/fileHelpers";

export interface PendingAttachment {
    localId: string;
    file: File;
    previewUrl: string | null;
    progress: number;
    status: "uploading" | "uploaded" | "error";
    error: string | null;
    uploaded: UploadedFile | null;
}

/**
 * Keeps the list of files attached to an unsent message and uploads them in parallel.
 * The list order is preserved, so the message attachments keep the order the user picked.
 */
export const useAttachmentUploads = (kind: FileKind = "attachment") => {
    const [attachments, setAttachments] = useState<PendingAttachment[]>([]);
    const [rejection, setRejection] = useState<string | null>(null);
    const attachmentsRef = useRef<PendingAttachment[]>([]);

    const apply = useCallback((updater: (current: PendingAttachment[]) => PendingAttachment[]) => {
        attachmentsRef.current = updater(attachmentsRef.current);
        setAttachments(attachmentsRef.current);
    }, []);

    const patch = useCallback((localId: string, values: Partial<PendingAttachment>) => {
        apply(current => current.map(item => item.localId === localId ? { ...item, ...values } : item));
    }, [apply]);

    const startUpload = useCallback((pending: PendingAttachment) => {
        uploadFile(pending.file, kind, (progress) => patch(pending.localId, { progress }))
            .then((uploaded) => patch(pending.localId, { status: "uploaded", progress: 100, uploaded }))
            .catch((error: Error) => patch(pending.localId, { status: "error", error: error.message }));
    }, [kind, patch]);

    const addFiles = useCallback((files: File[]) => {
        const problems: string[] = [];
        const accepted: PendingAttachment[] = [];
        const room = Math.max(0, MAX_ATTACHMENTS - attachmentsRef.current.length);

        files.forEach((file, index) => {
            const problem = validateFile(file, kind);
            if (problem) {
                problems.push(problem);
                return;
            }
            if (accepted.length >= room) {
                problems.push(`You can attach up to ${MAX_ATTACHMENTS} files`);
                return;
            }

            accepted.push({
                localId: `${Date.now()}-${index}-${Math.random().toString(36).slice(2)}`,
                file,
                previewUrl: isImageFile(file) ? URL.createObjectURL(file) : null,
                progress: 0,
                status: "uploading",
                error: null,
                uploaded: null,
            });
        });

        setRejection(problems[0] ?? null);

        if (accepted.length === 0)
            return;

        apply(current => [...current, ...accepted]);

        accepted.forEach(startUpload);
    }, [apply, kind, startUpload]);

    const remove = useCallback((localId: string) => {
        apply(current => {
            const target = current.find(item => item.localId === localId);
            if (target?.previewUrl)
                URL.revokeObjectURL(target.previewUrl);

            return current.filter(item => item.localId !== localId);
        });
    }, [apply]);

    const clear = useCallback(() => {
        attachmentsRef.current.forEach(item => {
            if (item.previewUrl)
                URL.revokeObjectURL(item.previewUrl);
        });

        attachmentsRef.current = [];
        setAttachments([]);
        setRejection(null);
    }, []);

    useEffect(() => () => {
        attachmentsRef.current.forEach(item => {
            if (item.previewUrl)
                URL.revokeObjectURL(item.previewUrl);
        });
    }, []);

    const isUploading = attachments.some(item => item.status === "uploading");
    const hasErrors = attachments.some(item => item.status === "error");
    const uploaded = attachments
        .map(item => item.uploaded)
        .filter((item): item is UploadedFile => item !== null);

    return {
        attachments,
        uploaded,
        isUploading,
        hasErrors,
        rejection,
        clearRejection: () => setRejection(null),
        addFiles,
        remove,
        clear,
    };
};
