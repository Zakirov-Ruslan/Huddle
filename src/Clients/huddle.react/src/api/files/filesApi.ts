import { GATEWAY_URL } from "../api";
import getUser, { getSessionId } from "../../utils/authHelpers";

export const FILES_SERVICE_URL = `${GATEWAY_URL}/files`;

export type FileKind = "attachment" | "avatar";

export interface UploadedFile {
    id: string;
    url: string;
    fileName: string;
    contentType: string;
    size: number;
    isImage: boolean;
}

/** Turns the API relative file path (`/files/api/files/{id}/content`) into an absolute URL. */
export const resolveFileUrl = (url: string): string =>
    url.startsWith("http") ? url : `${GATEWAY_URL}${url}`;

export const uploadFile = (
    file: File,
    kind: FileKind,
    onProgress?: (progress: number) => void
): Promise<UploadedFile> => {
    return new Promise((resolve, reject) => {
        const user = getUser();
        const token = user?.access_token;

        if (!token) {
            reject(new Error("No token provided"));
            return;
        }

        const xhr = new XMLHttpRequest();
        xhr.open("POST", `${FILES_SERVICE_URL}/api/files?kind=${kind}`);
        xhr.setRequestHeader("Authorization", `Bearer ${token}`);

        const sessionId = getSessionId();
        if (sessionId)
            xhr.setRequestHeader("X-Session-Id", sessionId);

        xhr.upload.onprogress = (event) => {
            if (event.lengthComputable && onProgress)
                onProgress(Math.round((event.loaded / event.total) * 100));
        };

        xhr.onload = () => {
            if (xhr.status >= 200 && xhr.status < 300) {
                resolve(JSON.parse(xhr.responseText) as UploadedFile);
                return;
            }

            reject(new Error(xhr.responseText || `Upload failed with status ${xhr.status}`));
        };

        xhr.onerror = () => reject(new Error("Upload failed"));

        const formData = new FormData();
        formData.append("file", file);

        xhr.send(formData);
    });
};
