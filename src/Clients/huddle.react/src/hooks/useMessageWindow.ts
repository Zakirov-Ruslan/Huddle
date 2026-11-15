import { useCallback, useEffect, useLayoutEffect, useMemo, useRef, useState } from "react";
import { useTextChannelStore, type LocalMessage } from "../stores/textChannelStore";

/** Messages rendered around the viewport. */
const WINDOW_SIZE = 100;
/** How many messages are appended to the window when the user reaches its edge. */
const EXTEND_CHUNK = 30;
/** Hard limit of rendered messages, everything else stays in the query cache. */
const MAX_RENDERED = 160;
/** Distance to the top/bottom edge that moves the window or loads another page. */
const EDGE_TRIGGER = 600;
/** Distance to the bottom that still counts as "the user is at the bottom". */
const BOTTOM_TOLERANCE = 80;
/** Minimum interval between position saves while scrolling. */
const POSITION_SAVE_INTERVAL = 150;
/** Duration of the smooth scroll to the bottom, used to avoid fighting it with resize snapping. */
const SMOOTH_SCROLL_GUARD = 700;

interface Anchor {
    id: string;
    offset: number;
}

type PendingScroll =
    | { type: "bottom"; smooth: boolean }
    | { type: "anchor"; id: string; offset: number };

interface UseMessageWindowOptions {
    channelId: string;
    /** Full sorted list of already loaded messages (server pages + optimistic ones). */
    messages: LocalMessage[];
    hasOlder: boolean;
    isFetchingOlder: boolean;
    onLoadOlder: () => void;
}

const findMessageElement = (container: HTMLElement, id: string) =>
    container.querySelector<HTMLElement>(`[data-message-id="${id}"]`);

/** First message intersecting the top of the viewport, with its offset inside the container. */
const captureAnchor = (container: HTMLElement): Anchor | null => {
    const containerTop = container.getBoundingClientRect().top;
    const nodes = container.querySelectorAll<HTMLElement>("[data-message-id]");

    for (const node of nodes) {
        const rect = node.getBoundingClientRect();

        if (rect.bottom > containerTop + 1) {
            return { id: node.dataset.messageId!, offset: rect.top - containerTop };
        }
    }

    return null;
};

const scrollToAnchor = (container: HTMLElement, id: string, offset: number) => {
    const element = findMessageElement(container, id);
    if (!element)
        return;

    const delta = element.getBoundingClientRect().top - container.getBoundingClientRect().top - offset;

    if (Math.abs(delta) > 0.5)
        container.scrollTop += delta;
};

const scrollContainerToBottom = (container: HTMLElement, smooth: boolean) => {
    container.scrollTo({ top: container.scrollHeight, behavior: smooth ? "smooth" : "auto" });
};

/**
 * Keeps only a window of messages in the DOM while the rest of the loaded history stays in the
 * query cache, restores the scroll position per channel and handles "stick to bottom" behaviour.
 */
export const useMessageWindow = ({
    channelId,
    messages,
    hasOlder,
    isFetchingOlder,
    onLoadOlder,
}: UseMessageWindowOptions) => {
    const containerRef = useRef<HTMLDivElement | null>(null);
    const contentRef = useRef<HTMLDivElement | null>(null);

    const [initialized, setInitialized] = useState(false);
    const [windowStartId, setWindowStartId] = useState<string | null>(null);
    const [windowEndId, setWindowEndId] = useState<string | null>(null);
    const [, forceRender] = useState(0);
    const [isAtBottom, setIsAtBottom] = useState(true);
    const [hasNewMessages, setHasNewMessages] = useState(false);

    const isAtBottomRef = useRef(true);
    const pendingAnchorRef = useRef<Anchor | null>(null);
    const pendingScrollRef = useRef<PendingScroll | null>(null);
    const smoothScrollUntilRef = useRef(0);
    const lastMessageIdRef = useRef<string | null>(null);
    const lastLengthRef = useRef(0);
    const lastSaveAtRef = useRef(0);
    const frameRef = useRef<number | null>(null);
    const olderRequestRef = useRef(false);

    useEffect(() => {
        setInitialized(false);
        setWindowStartId(null);
        setWindowEndId(null);
        setIsAtBottom(true);
        setHasNewMessages(false);

        isAtBottomRef.current = true;
        pendingAnchorRef.current = null;
        pendingScrollRef.current = null;
        smoothScrollUntilRef.current = 0;
        lastMessageIdRef.current = null;
        lastLengthRef.current = 0;
        lastSaveAtRef.current = 0;
        olderRequestRef.current = false;
    }, [channelId]);

    const startIndex = useMemo(() => {
        if (!initialized || messages.length === 0)
            return 0;

        const index = windowStartId ? messages.findIndex(message => message.id === windowStartId) : -1;

        return index >= 0 ? index : Math.max(0, messages.length - WINDOW_SIZE);
    }, [initialized, messages, windowStartId]);

    const endIndex = useMemo(() => {
        if (!initialized || messages.length === 0)
            return -1;

        const index = windowEndId ? messages.findIndex(message => message.id === windowEndId) : -1;

        return index >= 0 ? index : messages.length - 1;
    }, [initialized, messages, windowEndId]);

    const visibleMessages = useMemo(() => {
        if (!initialized || endIndex < 0)
            return [];

        return messages.slice(startIndex, Math.max(startIndex, endIndex) + 1);
    }, [initialized, messages, startIndex, endIndex]);

    const savePosition = useCallback(() => {
        const container = containerRef.current;
        if (!container)
            return;

        const now = Date.now();
        if (now - lastSaveAtRef.current < POSITION_SAVE_INTERVAL)
            return;

        lastSaveAtRef.current = now;
        const anchor = captureAnchor(container);

        useTextChannelStore.getState().setPosition(channelId, {
            messageId: anchor?.id ?? null,
            offset: anchor ? Math.round(anchor.offset) : 0,
            atBottom: isAtBottomRef.current,
        });
    }, [channelId]);

    // First render for the channel: restore the saved anchor or jump to the newest message.
    useEffect(() => {
        if (initialized || messages.length === 0)
            return;

        const saved = useTextChannelStore.getState().getPosition(channelId);
        const savedIndex = saved?.messageId
            ? messages.findIndex(message => message.id === saved.messageId)
            : -1;

        if (saved && saved.messageId && savedIndex >= 0) {
            const start = Math.max(0, Math.min(savedIndex - Math.floor(WINDOW_SIZE / 2), messages.length - WINDOW_SIZE));
            const end = Math.min(messages.length - 1, start + WINDOW_SIZE - 1);

            pendingScrollRef.current = { type: "anchor", id: saved.messageId, offset: saved.offset };
            setWindowStartId(messages[start].id);
            setWindowEndId(messages[end].id);
            isAtBottomRef.current = saved.atBottom;
            setIsAtBottom(saved.atBottom);
        } else {
            const start = Math.max(0, messages.length - WINDOW_SIZE);

            pendingScrollRef.current = { type: "bottom", smooth: false };
            setWindowStartId(messages[start].id);
            setWindowEndId(messages[messages.length - 1].id);
            isAtBottomRef.current = true;
            setIsAtBottom(true);
        }

        lastMessageIdRef.current = messages[messages.length - 1].id;
        lastLengthRef.current = messages.length;
        forceRender(request => request + 1);
        setInitialized(true);
    }, [channelId, initialized, messages]);

    // Window changes and page loads: keep the anchor message glued to its viewport offset,
    // follow the newest message while the user is at the bottom and extend the window on top.
    useLayoutEffect(() => {
        const container = containerRef.current;
        if (!container)
            return;

        const pending = pendingScrollRef.current;

        if (pending) {
            pendingScrollRef.current = null;

            if (pending.type === "bottom") {
                if (pending.smooth)
                    smoothScrollUntilRef.current = Date.now() + SMOOTH_SCROLL_GUARD;

                scrollContainerToBottom(container, pending.smooth);
            } else {
                scrollToAnchor(container, pending.id, pending.offset);
            }

            return;
        }

        const anchor = pendingAnchorRef.current;
        if (anchor) {
            pendingAnchorRef.current = null;
            scrollToAnchor(container, anchor.id, anchor.offset);
        }
    });

    useLayoutEffect(() => {
        const container = containerRef.current;
        if (!container || !initialized || messages.length === 0)
            return;

        const lastId = messages[messages.length - 1].id;
        const previousLastId = lastMessageIdRef.current;
        const previousLength = lastLengthRef.current;

        lastMessageIdRef.current = lastId;
        lastLengthRef.current = messages.length;

        // Older page arrived: keep the history flowing above the viewport.
        if (messages.length > previousLength && startIndex > 0 && container.scrollTop <= EDGE_TRIGGER) {
            pendingAnchorRef.current = captureAnchor(container);
            const newStart = Math.max(0, startIndex - EXTEND_CHUNK);
            setWindowStartId(messages[newStart].id);
        }

        if (!previousLastId || previousLastId === lastId)
            return;

        if (isAtBottomRef.current) {
            setWindowEndId(lastId);
            pendingScrollRef.current = { type: "bottom", smooth: true };
            forceRender(request => request + 1);
        } else {
            setHasNewMessages(true);
        }
    }, [initialized, messages, startIndex]);

    const expandUp = useCallback(() => {
        const container = containerRef.current;
        if (!container || !initialized || messages.length === 0)
            return;

        if (startIndex > 0) {
            pendingAnchorRef.current = captureAnchor(container);

            const newStart = Math.max(0, startIndex - EXTEND_CHUNK);
            const newEnd = Math.max(newStart, Math.min(endIndex, newStart + MAX_RENDERED - 1));

            setWindowStartId(messages[newStart].id);
            setWindowEndId(messages[newEnd].id);
            return;
        }

        if (hasOlder && !isFetchingOlder && !olderRequestRef.current) {
            olderRequestRef.current = true;
            onLoadOlder();
        }
    }, [endIndex, hasOlder, initialized, isFetchingOlder, messages, onLoadOlder, startIndex]);

    const expandDown = useCallback(() => {
        const container = containerRef.current;
        if (!container || !initialized || messages.length === 0)
            return;

        if (endIndex >= messages.length - 1)
            return;

        pendingAnchorRef.current = captureAnchor(container);

        const newEnd = Math.min(messages.length - 1, endIndex + EXTEND_CHUNK);
        const newStart = Math.max(0, Math.min(startIndex, newEnd - MAX_RENDERED + 1));

        setWindowStartId(messages[newStart].id);
        setWindowEndId(messages[newEnd].id);
    }, [endIndex, initialized, messages, startIndex]);

    const onScroll = useCallback(() => {
        if (frameRef.current !== null)
            return;

        frameRef.current = requestAnimationFrame(() => {
            frameRef.current = null;

            const container = containerRef.current;
            if (!container)
                return;

            const distanceToBottom = container.scrollHeight - container.scrollTop - container.clientHeight;
            const atBottom = distanceToBottom <= BOTTOM_TOLERANCE;

            if (atBottom !== isAtBottomRef.current) {
                isAtBottomRef.current = atBottom;
                setIsAtBottom(atBottom);
            }

            if (atBottom)
                setHasNewMessages(current => current ? false : current);

            savePosition();

            if (container.scrollTop < EDGE_TRIGGER)
                expandUp();

            if (distanceToBottom < EDGE_TRIGGER)
                expandDown();
        });
    }, [expandDown, expandUp, savePosition]);

    const scrollToBottom = useCallback((smooth = true) => {
        if (messages.length === 0)
            return;

        const start = Math.max(0, messages.length - WINDOW_SIZE);

        setWindowStartId(messages[start].id);
        setWindowEndId(messages[messages.length - 1].id);
        setHasNewMessages(false);
        setIsAtBottom(true);
        isAtBottomRef.current = true;
        pendingScrollRef.current = { type: "bottom", smooth };
        forceRender(request => request + 1);
    }, [messages]);

    // Keep the view pinned to the bottom while images and attachments change the content height.
    useEffect(() => {
        const container = containerRef.current;
        const content = contentRef.current;

        if (!container || !content || typeof ResizeObserver === "undefined")
            return;

        const observer = new ResizeObserver(() => {
            if (!isAtBottomRef.current || pendingScrollRef.current)
                return;

            if (Date.now() < smoothScrollUntilRef.current)
                return;

            const distanceToBottom = container.scrollHeight - container.scrollTop - container.clientHeight;

            if (distanceToBottom > 0 && distanceToBottom <= BOTTOM_TOLERANCE + 40)
                container.scrollTop = container.scrollHeight;
        });

        observer.observe(content);

        return () => observer.disconnect();
    }, []);

    useEffect(() => {
        olderRequestRef.current = false;
    }, [messages.length, hasOlder]);

    useEffect(() => () => {
        if (frameRef.current !== null)
            cancelAnimationFrame(frameRef.current);
    }, []);

    return {
        containerRef,
        contentRef,
        visibleMessages,
        startIndex,
        endIndex,
        isAtBottom,
        hasNewMessages,
        onScroll,
        scrollToBottom,
    };
};

