import type { MessageDto } from "../api/dtos";
import { useUserProfile } from "../hooks/usersApiHooks";

interface AuthorMessageGroupProps {
    authorId: string;
    messageGroup: MessageDto[];
}

const AuthorMessageGroup = ({ authorId, messageGroup }: AuthorMessageGroupProps) => {

    const { data: profile, isLoading: isProfileLoading, error: isProfileError } = useUserProfile(authorId);

    return (
        <div className="mb-4">
            {messageGroup.map((msg, msgIndex) => (

                <div
                    key={msg.id}
                    className={`flex items-start space-x-2 ${msgIndex === 0 ? 'mt-2' : 'mt-1'
                        }`}
                >
                    {msgIndex === 0 ? (
                        <>
                            <div className="flex h-8 w-8 flex-shrink-0 items-center justify-center rounded-full bg-blue-500 text-sm font-medium text-white">
                                {profile?.userName.charAt(0).toUpperCase()}
                            </div>
                            <div className="flex-1">
                                <div className="flex items-center space-x-2">
                                    <span className="font-medium text-gray-800 dark:text-slate-200">
                                        {profile?.userName}
                                    </span>
                                </div>
                                <div className="flex flex-row">
                                    <p className="mt-1 flex-grow text-left text-gray-700 dark:text-slate-200">
                                        {msg.text}
                                    </p>
                                    <span className="text-[12px] text-gray-500 dark:text-gray-400">
                                        {new Date(msg.sentAt).toLocaleTimeString([], {
                                            hour: '2-digit',
                                            minute: '2-digit',
                                        })}
                                    </span>
                                </div>

                            </div>
                        </>
                    ) : (
                        <div className="ml-10 flex flex-1 flex-row">
                                <p className="mt-1 flex-grow text-left text-gray-700 dark:text-slate-200">
                                    {msg.text}
                                </p>
                                <div className="flex items-center justify-end">
                                    <span className="text-[12px] text-gray-500 dark:text-gray-400">
                                        {new Date(msg.sentAt).toLocaleTimeString([], {
                                            hour: '2-digit',
                                            minute: '2-digit',
                                        })}
                                    </span>
                                </div>
                        </div>
                    )}
                </div>
            ))}
        </div>
    );
};

export default AuthorMessageGroup;