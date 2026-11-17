import { Skeleton, SkeletonText } from "./Skeleton";

/**
 * Composed skeletons for the screens that load data from the API.
 * They mirror the real layout, so the content does not jump when it arrives.
 */

/** Chat history: avatar + author line + message lines, repeated. */
export const MessageListSkeleton = ({ groups = 4 }: { groups?: number }) => (
    <div className="flex flex-col gap-5" role="status" aria-label="Loading messages">
        {Array.from({ length: groups }).map((_, index) => (
            <div key={index} className="flex gap-3">
                <Skeleton shape="circle" className="mt-1 h-8 w-8 shrink-0" />
                <div className="min-w-0 flex-1">
                    <Skeleton shape="text" className="mb-2 h-3.5 w-32" />
                    <SkeletonText lines={index % 2 === 0 ? 2 : 1} lineClassName="h-3.5" />
                </div>
            </div>
        ))}
    </div>
);

/** Members panel: avatar + name rows. */
export const MemberListSkeleton = ({ rows = 7 }: { rows?: number }) => {
    // Varying widths read as a real list rather than a stack of identical bars.
    const widths = ["w-28", "w-20", "w-32", "w-24", "w-16", "w-28", "w-24"];

    return (
        <div className="flex flex-col gap-1" role="status" aria-label="Loading members">
            {Array.from({ length: rows }).map((_, index) => (
                <div key={index} className="flex items-center gap-2 rounded-md p-1.5">
                    <Skeleton shape="circle" className="h-8 w-8 shrink-0" />
                    <Skeleton shape="text" className={`h-3.5 ${widths[index % widths.length]}`} />
                </div>
            ))}
        </div>
    );
};

/** Voice channel stage: participant tiles. */
export const VoiceStageSkeleton = ({ tiles = 3 }: { tiles?: number }) => (
    <div
        className="flex min-h-0 w-full flex-1 flex-wrap items-center justify-center gap-3 p-4 md:gap-10 md:p-10"
        role="status"
        aria-label="Loading voice channel"
    >
        {Array.from({ length: tiles }).map((_, index) => (
            <Skeleton key={index} className="aspect-video w-32 shrink-0 md:w-64" />
        ))}
    </div>
);

/** Settings pages: sidebar entry + form column. */
export const SettingsSkeleton = () => (
    <div className="flex h-full w-full flex-col gap-4 p-6 md:flex-row md:gap-8" role="status" aria-label="Loading settings">
        <div className="w-full md:w-1/3">
            <Skeleton shape="text" className="mb-4 h-4 w-40" />
            <div className="flex flex-col gap-2">
                <Skeleton shape="text" className="h-4 w-48" />
                <Skeleton shape="text" className="h-4 w-36" />
            </div>
        </div>
        <div className="flex min-w-0 flex-1 flex-col gap-6">
            <div className="flex items-center gap-4">
                <Skeleton className="h-20 w-20" />
                <div className="flex flex-1 flex-col gap-2">
                    <Skeleton shape="text" className="h-4 w-52" />
                    <Skeleton shape="text" className="h-3 w-32" />
                </div>
            </div>
            <Skeleton className="h-10 w-full max-w-md" />
            <Skeleton className="h-10 w-full max-w-md" />
            <Skeleton className="h-9 w-28" />
        </div>
    </div>
);

/** Whole channel screen: header + messages, used while the server itself is loading. */
export const ChatSkeleton = () => (
    <div className="flex min-h-0 flex-1 flex-col" role="status" aria-label="Loading channel">
        <div className="flex h-[50px] shrink-0 items-center border-b border-gray-300 px-4">
            <Skeleton shape="text" className="h-4 w-36" />
        </div>
        <div className="min-h-0 flex-1 overflow-hidden p-4 md:p-6">
            <MessageListSkeleton groups={6} />
        </div>
    </div>
);

/** Application shell while the session is being restored (rail + sidebar + chat). */
export const AppShellSkeleton = () => (
    <div className="flex h-full w-full overflow-hidden bg-gray-100" role="status" aria-label="Loading application">
        <div className="flex w-16 shrink-0 flex-col items-center gap-3 bg-gray-100 pt-4 lg:w-24">
            {Array.from({ length: 4 }).map((_, index) => (
                <Skeleton key={index} className="h-11 w-11 lg:h-15 lg:w-15" />
            ))}
        </div>
        <div className="hidden w-60 shrink-0 flex-col gap-3 border-r border-gray-300 bg-gray-100 p-4 md:flex lg:w-[300px]">
            <Skeleton shape="text" className="mb-2 h-5 w-40" />
            {Array.from({ length: 3 }).map((_, index) => (
                <Skeleton key={index} shape="text" className="h-7 w-full" />
            ))}
            <Skeleton shape="text" className="mt-4 mb-2 h-4 w-28" />
            {Array.from({ length: 3 }).map((_, index) => (
                <Skeleton key={index} shape="text" className="h-7 w-full" />
            ))}
        </div>
        <div className="flex min-w-0 flex-1 flex-col bg-white md:m-2 md:rounded-xl">
            <div className="flex h-[50px] shrink-0 items-center border-b border-gray-200 px-4">
                <Skeleton shape="text" className="h-4 w-32" />
            </div>
            <div className="min-h-0 flex-1 overflow-hidden p-4 md:p-6">
                <MessageListSkeleton groups={5} />
            </div>
        </div>
    </div>
);
