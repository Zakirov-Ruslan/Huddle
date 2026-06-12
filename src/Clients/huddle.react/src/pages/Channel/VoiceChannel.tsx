import { LiveKitRoom, useTracks, ParticipantTile, GridLayout, ControlBar, useParticipants, RoomAudioRenderer, useIsSpeaking } from "@livekit/components-react";
import type { Channel } from "../../api/types";
import { useLiveKitToken } from "../../hooks/queries/livekit";
import { useEffect } from "react";
import { useUserProfile } from "../../hooks/queries/users";
import { LocalParticipant, RemoteParticipant, Track } from "livekit-client";
import { useVoiceChannelStore } from "../../stores/voiceChannelStore";

const ParticipantPlaceholder: React.FC<{ participant: LocalParticipant | RemoteParticipant }> = ({ participant }) => {

    const userId = participant.identity;
    const { data: profile, isLoading } = useUserProfile(userId); 
    const isSpeaking = useIsSpeaking(participant);

    return (
        <div
            className={`
                h-100 rounded-xl aspect-video
                bg-gradient-to-br from-[#71787f] to-[#57636d]
                flex flex-col items-center justify-center
                text-white text-sm font-bold
                p-2 text-center break-words
                transition-all duration-100 ease-out
                ${isSpeaking && "border-3 border-green-400 shadow-[0_0_15px_rgba(74,222,128,0.5)]"}
            `}
        >
            {/* Иконка */}
            <div className="mb-1 text-2xl">
                {isSpeaking ? "🎤" : "👤"}
            </div>

            {/* Имя пользователя */}
            {isLoading ? (
                <span className="opacity-70">Loading...</span>
            ) : (
                <span>{profile?.userName || userId}</span>
            )}
        </div>
    );
};

// Сетка участников
const VoiceChannelUI: React.FC = () => {

    const participants = useParticipants();

    return (
            <div className="flex w-full flex-wrap items-center justify-center gap-10 overflow-hidden p-10">
                {participants.map((p) => (
                        <ParticipantPlaceholder key={p.identity} participant={p} />
                ))}
            </div>
    );
};

const VoiceChannel: React.FC<Channel> = ({ id, serverId, name, channelType }) => {

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

    if (isLoading) return <div>Loading...</div>;
    if (error || !token) return <div>Getting token error</div>;

    if (activeChannel?.id === id) {
        return (
            <VoiceChannelUI />
        );
    }

    return (
        <div className="flex h-full w-full items-center justify-center">
            <div className="flex flex-col gap-2">
                <span className="text-3xl font-medium">{name}</span>
                <button
                    className="text rounded-md bg-gray-200 px-4 py-2 font-medium"
                    onClick={handleJoinChannel}
                >
                    Join voice channel
                </button>
            </div>
        </div>
    );
};

export default VoiceChannel;