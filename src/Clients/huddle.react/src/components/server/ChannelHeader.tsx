import { FaArrowLeft, FaHashtag, FaUsers } from "react-icons/fa";
import { HiSpeakerWave } from "react-icons/hi2";
import { IoClose } from "react-icons/io5";
import type { Channel } from "../../api/types";
import { AppTooltip } from "../AppTooltip";
import { Skeleton } from "../skeleton/Skeleton";

interface ChannelHeaderProps {
    channel?: Channel | null;
    isMembersPanelOpened: boolean;
    onToggleMembers: () => void;
    /** Goes back to the channel list — only visible while the chat is a separate screen. */
    onBack: () => void;
}

const ChannelHeader = ({ channel, isMembersPanelOpened, onToggleMembers, onBack }: ChannelHeaderProps) => {
    const isTextChannel = channel?.channelType.toLowerCase() === "text";

    return (
        <header className="flex h-[50px] shrink-0 items-center justify-between gap-2 border-b border-gray-300 bg-white px-2 md:px-4">
            <div className="flex min-w-0 items-center gap-1">
                <button
                    type="button"
                    onClick={onBack}
                    aria-label="Back to channels"
                    className="flex h-8 w-8 shrink-0 items-center justify-center rounded-md text-gray-600 transition-colors duration-150 hover:bg-gray-200 md:hidden"
                >
                    <FaArrowLeft />
                </button>

                {channel ? (
                    <h2 className="flex min-w-0 items-center gap-1 font-semibold text-gray-700">
                        {isTextChannel ? <FaHashtag className="shrink-0" /> : <HiSpeakerWave className="shrink-0" />}
                        <span className="truncate">{channel.name}</span>
                    </h2>
                ) : (
                    <Skeleton shape="text" className="h-4 w-32" />
                )}
            </div>

            <div className="flex shrink-0 items-center gap-1">
                <button
                    type="button"
                    onClick={onToggleMembers}
                    aria-label={isMembersPanelOpened ? "Hide list of members" : "Show list of members"}
                    className="flex h-8 w-8 items-center justify-center rounded-md transition-colors duration-150 hover:bg-gray-200"
                    data-tooltip-id="members-panel-switcher"
                >
                    {isMembersPanelOpened && isTextChannel
                        ? <IoClose />
                        : <FaUsers />}
                </button>
                <AppTooltip id="members-panel-switcher" place="bottom-end">
                    {isMembersPanelOpened ? "Hide list of members" : "Show list of members"}
                </AppTooltip>
            </div>
        </header>
    );
};

export default ChannelHeader;
