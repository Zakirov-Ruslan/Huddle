import { Tooltip } from "react-tooltip";
import type { Message } from "../api/types";
import { useUserProfile } from "../hooks/queries/users";
import type { LocalMessage } from "../stores/textChannelStore";
import MessageAttachments from "./chat/MessageAttachments";

interface AuthorMessageGroupProps {
    authorId: string;
    messageGroup: LocalMessage[];
}

const AuthorMessageGroup = ({ authorId, messageGroup }: AuthorMessageGroupProps) => {

    const { data: profile, isLoading: isProfileLoading, error: isProfileError } = useUserProfile(authorId);

    return (
        <div className="relative mb-4">
            <div className="absolute top-0 bottom-0 left-1 flex flex-col-reverse">
                <div className="sticky! bottom-1 select-none">
                    <div
                        className="flex h-8 w-8 flex-shrink-0 items-center justify-center rounded-full bg-[#5D6D7B] text-sm font-medium text-white"
                        data-tooltip-id={`message-avatart-item-tooltip-${messageGroup[0]?.id}`}
                    >
                        {profile?.userName.charAt(0).toUpperCase()}
                    </div>

                    <Tooltip
                        id={`message-avatart-item-tooltip-${messageGroup[0]?.id}`} data-tooltip-content={profile?.userName}
                        style={{
                            backgroundColor: "rgb(255, 255, 255)", color: "#222", borderRadius: "10px", fontWeight: "500", padding: "5px 10px 8px 10px", boxShadow: "0 20px 25px -5px rgb(0 0 0 / 0.1)"
                        }}
                        opacity={1}
                        border="1px solid #e8e8e8"
                        place="top"
                    >
                        {profile?.userName}
                    </Tooltip>
                </div>
            </div>
            <div className="ml-12 flex min-w-0 flex-col">
                {messageGroup.map((msg, msgIndex) => (
                    <div
                        key={msg.id}
                        data-message-id={msg.id}
                        className={`flex hover:bg-gray-100 rounded-md px-4 pb-2 items-start space-x-2 min-w-0 ${msgIndex === 0 ? 'mt-1' : 'mt-0'}`}
                    >
                        <div className="min-w-0 flex-1">
                            {msgIndex == 0 &&
                                <div className="flex items-center space-x-2">
                                    <span className="font-medium text-gray-800 dark:text-slate-200">
                                        {profile?.userName}
                                    </span>
                                </div>
                            }
                            <div className="flex min-w-0 flex-row align-middle">
                                {msg.text.length > 0 && (
                                    <p className={`mt-1 flex-grow text-left whitespace-pre-line ${msg.status == "pending"
                                        ? "text-gray-400"
                                        : msg.status === "error"
                                            ? "text-red-400"
                                            : "text-gray-700"
                                        }`}>
                                        {msg.text}
                                    </p>
                                )}
                                <span className="mt-2 ml-auto pl-2 text-[12px] text-gray-500 dark:text-gray-400">
                                    {new Date(msg.sentAt).toLocaleTimeString([], {
                                        hour: '2-digit',
                                        minute: '2-digit',
                                    })}
                                </span>
                            </div>

                            <MessageAttachments attachments={msg.attachments} />
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default AuthorMessageGroup;