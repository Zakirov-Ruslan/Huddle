import { useEffect } from "react";
import { useParams } from "react-router";
import { useAcceptInvite } from "../hooks/invitesApiHooks";
import { useNavigate } from "react-router";

function InviteRedirect() {

    const { inviteCode } = useParams();
    const acceptInvite = useAcceptInvite();
    const navigate = useNavigate();

    useEffect(() => {
        if (inviteCode) {
            acceptInvite.mutate(inviteCode, {
                onSuccess: (response) => {
                    navigate(`/s/${response.serverId}`) 
                }
            });

        }
    }, [inviteCode]);

    return (
        <>{inviteCode}</>
    );
}

export default InviteRedirect;