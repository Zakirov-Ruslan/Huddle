import { FaMicrophone } from "react-icons/fa";
import { RiVoiceprintLine } from "react-icons/ri";
import { TbDoorExit, TbHeadphonesOff } from "react-icons/tb";
import { useMyProfile } from "../../hooks/queries/users";
import { useServer } from "../../hooks/queries/servers";
import { useVoiceChannelStore } from "../../stores/voiceChannelStore";

interface UserPanelProps {
    className?: string;
}

/**
 * Current user + active voice connection (voice status, leave, mic, headphones).
 *
 * `Layout` renders it twice: as a normal row at the bottom of the main area on
 * phones/tablets and as the floating card on desktop. Same component, same
 * functionality — only the placement differs.
 */
const UserPanel = ({ className = "" }: UserPanelProps) => {
    const { data: profile } = useMyProfile();
    const { activeChannel, leaveChannel } = useVoiceChannelStore();
    const { data: voiceServer } = useServer(activeChannel?.serverId);

    const voiceChannelLabel = voiceServer?.name
        ? `${voiceServer.name} / ${activeChannel?.name}`
        : activeChannel?.name;

    return (
        <div className={`rounded-xl border border-gray-200 bg-white p-2 shadow-lg select-none lg:p-3 lg:shadow-2xl ${className}`}>
            {activeChannel && (
                <div className="mb-2 flex items-center gap-2 border-b border-gray-100 pb-2 text-left lg:mb-4 lg:gap-3 lg:border-0 lg:pb-0">
                    <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-md border border-green-600/40 bg-gray-100 text-green-600">
                        <RiVoiceprintLine />
                    </div>

                    <div className="flex min-w-0 flex-grow flex-col font-medium">
                        <span className="truncate text-sm text-green-600 lg:text-base">
                            Connected to the voice channel
                        </span>
                        <span className="truncate text-xs text-gray-600 lg:text-sm">
                            {voiceChannelLabel}
                        </span>
                    </div>

                    <button
                        type="button"
                        className="flex h-8 w-8 shrink-0 items-center justify-center rounded-md transition-colors duration-150 hover:bg-gray-100"
                        onClick={leaveChannel}
                        aria-label="Leave voice channel"
                    >
                        <TbDoorExit />
                    </button>
                </div>
            )}

            <div className="flex flex-row items-center gap-1">
                <div className="flex min-w-0 flex-grow items-center gap-2 rounded-lg px-1 transition-colors duration-150 hover:bg-gray-100">
                    <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-[#5D6D7B] text-sm font-medium text-white lg:h-10 lg:w-10">
                        {profile?.userName?.[0]}
                    </div>
                    <span className="truncate font-medium">{profile?.userName}</span>
                </div>

                <button
                    type="button"
                    aria-label="Toggle microphone"
                    className="flex h-8 w-8 shrink-0 items-center justify-center rounded-md transition-colors duration-150 hover:bg-gray-100"
                >
                    <FaMicrophone />
                </button>
                <button
                    type="button"
                    aria-label="Toggle headphones"
                    className="flex h-8 w-8 shrink-0 items-center justify-center rounded-md transition-colors duration-150 hover:bg-gray-100"
                >
                    <TbHeadphonesOff />
                </button>
            </div>
        </div>
    );
};

export default UserPanel;
