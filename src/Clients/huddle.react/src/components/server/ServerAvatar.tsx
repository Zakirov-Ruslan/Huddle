import { useEffect, useState } from "react";
import { resolveFileUrl } from "../../api/files/filesApi";

interface ServerAvatarProps {
    name: string;
    avatarUrl?: string | null;
    /** Classes applied to the fallback letter. */
    className?: string;
    /** Extra classes applied to the image, e.g. rounding. */
    imageClassName?: string;
}

/** Server icon with a graceful fallback to the first letter of the name. */
const ServerAvatar = ({ name, avatarUrl, className = "", imageClassName = "" }: ServerAvatarProps) => {
    const [failed, setFailed] = useState(false);

    useEffect(() => {
        setFailed(false);
    }, [avatarUrl]);

    if (!avatarUrl || failed)
        return <span className={className}>{name.charAt(0).toUpperCase()}</span>;

    return (
        <img
            src={resolveFileUrl(avatarUrl)}
            alt={name}
            onError={() => setFailed(true)}
            className={`${className} ${imageClassName}`}
        />
    );
};

export default ServerAvatar;
