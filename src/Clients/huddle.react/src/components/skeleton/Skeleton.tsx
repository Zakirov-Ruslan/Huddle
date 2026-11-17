interface SkeletonProps {
    className?: string;
    shape?: "block" | "text" | "circle";
    /** `inverse` is for dark surfaces: voice tiles, brand-coloured avatars. */
    tone?: "default" | "inverse";
}

/** Shimmering placeholder block. Always `aria-hidden`: the meaning comes from the surrounding labels. */
export const Skeleton = ({ className = "", shape = "block", tone = "default" }: SkeletonProps) => (
    <div
        aria-hidden="true"
        className={`${tone === "inverse" ? "skeleton skeleton-inverse" : "skeleton"} ${shape === "circle" ? "rounded-full" : shape === "text" ? "rounded-md" : "rounded-xl"} ${className}`}
    />
);

interface SkeletonTextProps {
    lines?: number;
    className?: string;
    lineClassName?: string;
}

/** Stack of text lines with the last one shortened, like a paragraph placeholder. */
export const SkeletonText = ({ lines = 3, className = "", lineClassName = "h-3" }: SkeletonTextProps) => (
    <div className={`flex flex-col gap-2 ${className}`} aria-hidden="true">
        {Array.from({ length: lines }).map((_, index) => (
            <Skeleton
                key={index}
                shape="text"
                className={`${lineClassName} ${index === lines - 1 ? "w-2/3" : "w-full"}`}
            />
        ))}
    </div>
);
