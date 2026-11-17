import { useEffect, useState } from "react";
import { useCreateInvite } from "../../hooks/queries/invites";
import Mascot from "../Mascot";
import { Skeleton } from "../skeleton/Skeleton";

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

            {createInvite.isError && (
                <div className="mt-3 flex items-center gap-3 rounded-xl bg-red-50 p-3">
                    <Mascot pose="angry" className="h-16" />
                    <div className="flex min-w-0 flex-col">
                        <span className="text-sm font-medium text-red-700">Could not create an invite</span>
                        <span className="text-xs text-red-500">Check your connection and try once more.</span>
                    </div>
                </div>
            )}

            <div className="mt-2 flex flex-row items-center gap-1 rounded-xl border-1 border-gray-400 bg-gray-200 px-2 py-1.5">
                {inviteLink ?
                    <span className="flex-grow text-sm font-medium">{inviteLink}</span> :
                    <Skeleton shape="text" className="h-5 min-w-60 flex-grow" />
                }
                <button
                    type="button"
className={` ml-2 rounded-md px-3 py-1 font-medium text-white ${isCopied ? 'bg-green-500' : 'bg-[#5D6D7B]'}` }
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