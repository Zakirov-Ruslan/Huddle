import { mascotPoses, type MascotPose } from "../img/mascot";

interface MascotProps {
    pose: MascotPose;
    /** Height in pixels; the width follows the aspect ratio. Omit it to size with `className`. */
    size?: number;
    className?: string;
    /** Decorative by default — the surrounding copy carries the meaning. */
    alt?: string;
}

/**
 * The microphone-headed mascot.
 *
 * <Mascot pose="waving" size={192} />
 * <Mascot pose="head" className="h-9 w-9 lg:h-12 lg:w-12" />
 */
const Mascot = ({ pose, size, className = "", alt = "" }: MascotProps) => (
    <img
        src={mascotPoses[pose]}
        alt={alt}
        aria-hidden={alt === "" || undefined}
        draggable={false}
        className={`pointer-events-none select-none ${className}`}
        style={size ? { height: size, width: "auto" } : undefined}
    />
);

export default Mascot;
