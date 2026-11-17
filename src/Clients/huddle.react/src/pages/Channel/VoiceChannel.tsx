import { useParticipants, useIsSpeaking, useLocalParticipant } from "@livekit/components-react";
import type { Channel } from "../../api/types";
import { useLiveKitToken } from "../../hooks/queries/livekit";
import { useEffect } from "react";
import { useUserProfile } from "../../hooks/queries/users";
import type { LocalParticipant, RemoteParticipant } from "livekit-client";
import { useVoiceChannelStore } from "../../stores/voiceChannelStore";
import Mascot from "../../components/Mascot";
import { Skeleton } from "../../components/skeleton/Skeleton";
import { VoiceStageSkeleton } from "../../components/skeleton/Skeletons";

const ParticipantPlaceholder: React.FC<{ participant: LocalParticipant | RemoteParticipant }> = ({ participant }) => {

    const userId = participant.identity;
    const { data: profile, isLoading } = useUserProfile(userId); 
    const isSpeaking = useIsSpeaking(participant);

    return (
        <div
            className={`
                aspect-video w-32 shrink-0 rounded-xl md:w-64
                bg-gradient-to-br from-[#71787f] to-[#57636d]
                flex flex-col items-center justify-center
                text-white text-sm font-bold
                p-2 text-center break-words
                transition-all duration-100 ease-out
                ${isSpeaking && "border-3 border-green-400 shadow-[0_0_15px_rgba(74,222,128,0.5)]"}
            `}
        >
            <Mascot
                pose={isSpeaking ? "speaking" : "head"}
                className="mb-1 h-12 w-auto"
            />

            {/* UserName */}
            {isLoading ? (
                <Skeleton shape="text" tone="inverse" className="h-3 w-24" />
            ) : (
                <span>{profile?.userName || userId}</span>
            )}
        </div>
    );
};

const VoiceChannelUI: React.FC = () => {

    const participants = useParticipants();
    const { localParticipant } = useLocalParticipant();

    useEffect(() => {
        if (localParticipant && !localParticipant.isMicrophoneEnabled) {
            localParticipant.setMicrophoneEnabled(true, {
                noiseSuppression: true,
                echoCancellation: true,
                autoGainControl: true,
            }).catch((err) => {
                console.error("Unable to turn on the microphone, the user may have disabled its use:", err);
            });
        }
    }, [localParticipant]);


    return (
        <div className="flex min-h-0 w-full flex-1 flex-wrap items-center justify-center gap-3 overflow-y-auto p-4 md:gap-10 md:p-10">
            {participants.map((p) => (
                <ParticipantPlaceholder key={p.identity} participant={p} />
            ))}
        </div>
    );
};

const VoiceChannel: React.FC<Channel> = ({ id, serverId, name }) => {

    const { data: token, error, isLoading } = useLiveKitToken(serverId, id);
    const { joinChannel, activeChannel } = useVoiceChannelStore();

    useEffect(() => {
        handleJoinChannel();
    }, [token]);

    const handleJoinChannel = () => {
        if (token && activeChannel?.id != id) {
            joinChannel({ id, serverId, name }, token);
        }
    }

    if (isLoading) return <VoiceStageSkeleton />;
    if (error || !token) return (
        <div className="flex flex-1 flex-col items-center justify-center gap-3 p-6 text-center">
            <Mascot pose="sad" alt="Sad Huddle mascot" className="h-40" />
            <span className="text-lg font-medium text-gray-700">Voice channel is unavailable</span>
            <span className="text-sm text-gray-500">We could not get a voice token. Try again in a moment.</span>
            <button
                className="rounded-md bg-[#5D6D7B] px-4 py-2 font-medium text-white transition-colors duration-150 hover:bg-[#4F5E6A]"
                onClick={() => window.location.reload()}
            >
                Reload
            </button>
        </div>
    );

    if (activeChannel?.id === id) {
        return (
            <VoiceChannelUI />
        );
    }

    return (
        <div className="flex min-h-0 w-full flex-1 items-center justify-center p-4">
            <div className="flex flex-col items-center gap-3 text-center">
                <Mascot pose="dancing" alt="Huddle mascot dancing" className="mascot-tilt h-44 md:h-52" />
                <span className="text-2xl font-medium md:text-3xl">{name}</span>
                <button
                    className="rounded-md bg-gray-200 px-4 py-2 font-medium"
                    onClick={handleJoinChannel}
                >
                    Join voice channel
                </button>
            </div>
        </div>
    );
};

export default VoiceChannel;