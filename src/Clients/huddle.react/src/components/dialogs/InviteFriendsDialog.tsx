import { useEffect, useState } from "react";
import { useCreateInvite } from "../../hooks/queries/invites";

interface InviteFriendsDialogProps {
    serverId: string;
}

const InviteFriendsDialog: React.FC<InviteFriendsDialogProps> = ({ serverId }) => {

    const createInvite = useCreateInvite(serverId);
    const [inviteLink, setInviteLink] = useState<string | null>(null);
    const [isCopied, setIsCopied] = useState<boolean>(false);

    useEffect(() => {
        const baseUrl = `${window.location.protocol}//${window.location.host}`;

        createInvite.mutate(
            { serverId: serverId },
            { onSuccess: (invite) => { setInviteLink(`${baseUrl}/invite/${invite.code}`)} }
        )
    }, [serverId])

    return (
        <div className="w-full max-w-md bg-white px-2 pb-2">
            <div className="mb-2 flex flex-row items-center text-left">
                <h3 className="flex-grow text-2xl font-bold text-gray-700">Invite friends to server</h3>
            </div>
            <span className="font-medium">Send invitatinal link to your friends</span>
            <div className="mt-2 flex flex-row items-center gap-1 rounded-xl border-1 border-gray-400 bg-gray-200 px-2 py-1.5">
                {inviteLink ?
                    <span className="flex-grow text-sm font-medium">{inviteLink}</span> :
                    <div className="h-5 min-w-60 animate-pulse rounded-md bg-gray-300"></div>
                }
                <button
                    type="button"
                    className={` ml-2 rounded-md px-3 py-1 font-medium text-white ${isCopied ? 'bg-green-500' : 'bg-indigo-500'}` }
                    onClick={() => {
                        if (inviteLink && inviteLink.trim().length > 0) {
                            navigator.clipboard.writeText(inviteLink)
                            setIsCopied(true);
                        }
                    }}
                >
                    {isCopied ? 'Copied' : 'Copy'}
                </button>
            </div>
        </div>
    );
}

export default InviteFriendsDialog;