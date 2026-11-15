import type { FileKind } from "../api/files/filesApi";

export const MAX_ATTACHMENTS = 10;
export const MAX_ATTACHMENT_SIZE_BYTES = 25 * 1024 * 1024;
export const MAX_AVATAR_SIZE_BYTES = 5 * 1024 * 1024;

export const ALLOWED_ATTACHMENT_TYPES = [
    "image/png",
    "image/jpeg",
    "image/gif",
    "image/webp",
    "application/pdf",
    "text/plain",
    "text/csv",
    "application/zip",
    "application/json",
    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
];

export const ALLOWED_AVATAR_TYPES = ["image/png", "image/jpeg", "image/gif", "image/webp"];

export const isImageFile = (file: { type: string }): boolean => file.type.startsWith("image/");

/** Mirrors the server-side rules so users see the problem before the upload starts. */
export const validateFile = (file: File, kind: FileKind): string | null => {
    const allowedTypes = kind === "avatar" ? ALLOWED_AVATAR_TYPES : ALLOWED_ATTACHMENT_TYPES;
    const maxSize = kind === "avatar" ? MAX_AVATAR_SIZE_BYTES : MAX_ATTACHMENT_SIZE_BYTES;

    if (file.size <= 0)
        return `${file.name}: file is empty`;

    if (file.size > maxSize)
        return `${file.name}: file is larger than ${formatBytes(maxSize)}`;

    if (!allowedTypes.includes(file.type))
        return `${file.name}: file type is not supported`;

    return null;
};

export const formatBytes = (bytes: number): string => {
    if (bytes < 1024)
        return `${bytes} B`;

    const units = ["KB", "MB", "GB"];
    let value = bytes / 1024;
    let unitIndex = 0;

    while (value >= 1024 && unitIndex < units.length - 1) {
        value /= 1024;
        unitIndex += 1;
    }

    return `${value.toFixed(value >= 10 ? 0 : 1)} ${units[unitIndex]}`;
};
