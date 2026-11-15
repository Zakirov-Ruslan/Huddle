import { useCallback, useRef, useState } from "react";

interface UseFileDropOptions {
    onFiles: (files: File[]) => void;
    enabled?: boolean;
}

/**
 * Drag & drop handling with a depth counter, so the overlay does not flicker
 * while the pointer moves over child elements.
 */
export const useFileDrop = ({ onFiles, enabled = true }: UseFileDropOptions) => {
    const [isDragging, setIsDragging] = useState(false);
    const dragDepth = useRef(0);

    const hasFiles = (event: React.DragEvent) =>
        Array.from(event.dataTransfer.types).includes("Files");

    const onDragEnter = useCallback((event: React.DragEvent) => {
        if (!enabled || !hasFiles(event))
            return;

        event.preventDefault();
        dragDepth.current += 1;
        setIsDragging(true);
    }, [enabled]);

    const onDragOver = useCallback((event: React.DragEvent) => {
        if (!enabled || !hasFiles(event))
            return;

        event.preventDefault();
        event.dataTransfer.dropEffect = "copy";
    }, [enabled]);

    const onDragLeave = useCallback((event: React.DragEvent) => {
        if (!enabled || !hasFiles(event))
            return;

        event.preventDefault();
        dragDepth.current = Math.max(0, dragDepth.current - 1);

        if (dragDepth.current === 0)
            setIsDragging(false);
    }, [enabled]);

    const onDrop = useCallback((event: React.DragEvent) => {
        if (!enabled)
            return;

        event.preventDefault();
        dragDepth.current = 0;
        setIsDragging(false);

        const files = Array.from(event.dataTransfer.files);
        if (files.length > 0)
            onFiles(files);
    }, [enabled, onFiles]);

    return { isDragging, dropHandlers: { onDragEnter, onDragOver, onDragLeave, onDrop } };
};
