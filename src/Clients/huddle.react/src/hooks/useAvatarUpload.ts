import { useRef, useState } from "react";
import { uploadFile, type UploadedFile } from "../api/files/filesApi";
import { ALLOWED_AVATAR_TYPES, validateFile } from "../utils/fileHelpers";

export const useAvatarUpload = (onUploaded: (file: UploadedFile) => void) => {
    const inputRef = useRef<HTMLInputElement>(null);
    const [progress, setProgress] = useState<number | null>(null);
    const [error, setError] = useState<string | null>(null);

    const start = async (file: File) => {
        const problem = validateFile(file, "avatar");
        if (problem) {
            setError(problem);
            return;
        }

        setError(null);
        setProgress(0);

        try {
            const uploaded = await uploadFile(file, "avatar", setProgress);
            onUploaded(uploaded);
        } catch (uploadError) {
            setError(uploadError instanceof Error ? uploadError.message : "Failed to upload the image");
        } finally {
            setProgress(null);
        }
    };

    const open = () => inputRef.current?.click();

    const fileInputProps = {
        ref: inputRef,
        type: "file",
        accept: ALLOWED_AVATAR_TYPES.join(","),
        className: "hidden",
        onChange: (event: React.ChangeEvent<HTMLInputElement>) => {
            const file = event.target.files?.[0];
            event.target.value = "";

            if (file)
                void start(file);
        },
    };

    return {
        open,
        start,
        fileInputProps,
        isUploading: progress !== null,
        progress,
        error,
        clearError: () => setError(null),
    };
};
