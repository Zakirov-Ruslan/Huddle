import type { VoiceParticipant } from "../../api/types";
import { useUserProfile } from "../../hooks/queries/users";
import { Skeleton } from "../skeleton/Skeleton";

interface VoiceParticipantsProps {
    participants: VoiceParticipant[];
}

/**
 * One row of the list: the profile name is fetched per user, exactly like the member list does,
 * so the same person is named identically everywhere in the application.
 */
const VoiceParticipantRow = ({ participant }: { participant: VoiceParticipant }) => {
    const { data: profile, isLoading } = useUserProfile(participant.userId);

    const name = profile?.userName ?? participant.displayName;

    return (
        <li className="flex min-w-0 items-center gap-2 rounded-lg px-2 py-1 text-sm text-gray-600">
            {isLoading ? (
                <Skeleton shape="circle" className="h-5 w-5 shrink-0" />
            ) : (
                <span className="flex h-5 w-5 shrink-0 items-center justify-center rounded-full bg-[#5D6D7B] text-[10px] font-medium text-white">
                    {name.charAt(0).toUpperCase()}
                </span>
            )}

            {isLoading
                ? <Skeleton shape="text" className="h-3.5 w-24" />
                : <span className="truncate">{name}</span>}
        </li>
    );
};

/**
 * Everyone sitting in a voice channel right now, listed under the channel name.
 * Read from the cached server snapshot, so it also shows the participants of channels
 * the current user is not connected to.
 */
const VoiceParticipants = ({ participants }: VoiceParticipantsProps) => {
    if (participants.length === 0)
        return null;

    return (
        <ul className="mt-0.5 mb-1 ml-7 flex flex-col gap-0.5">
            {participants.map(participant => (
                <VoiceParticipantRow key={participant.userId} participant={participant} />
            ))}
        </ul>
    );
};

export default VoiceParticipants;
