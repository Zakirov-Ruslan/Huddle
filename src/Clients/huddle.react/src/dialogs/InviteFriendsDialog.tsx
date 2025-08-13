import { useCreateInvite } from "../hooks/invitesApiHooks";
import { useEffect, useState } from "react";

interface InviteFriendsDialogProps {
    serverId: string;
}

const InviteFriendsDialog: React.FC<InviteFriendsDialogProps> = ({ serverId }) => {

    const createInvite = useCreateInvite(serverId);
    const [inviteLink, setInviteLink] = useState('');

    useEffect(() => {
        const baseUrl = `${window.location.protocol}//${window.location.host}`;

        createInvite.mutate(
            { serverId: serverId },
            { onSuccess: (invite) => { setInviteLink(`${baseUrl}/invite/${invite.code}`)} }
        )
    }, [serverId])

    return (
        <div className="w-full max-w-md bg-white p-8">
            <div className="mb-8 text-center">
                <h1 className="mb-2 text-2xl font-bold text-gray-800">Invite someone please...</h1>
                {inviteLink}
            </div>
        </div>
    );
}

export default InviteFriendsDialog;