import { useEffect, useRef } from "react";
import { useParams, useNavigate } from "react-router";
import { v4 as uuidv4 } from 'uuid';
import { useAcceptInvite } from "../hooks/queries/invites";


function InviteRedirect() {
    const { inviteCode } = useParams<{ inviteCode?: string }>();
    const navigate = useNavigate();
    const acceptInvite = useAcceptInvite();
    const requestIdRef = useRef<string | null>(null);

    useEffect(() => {
        if (!inviteCode) return;

        // Генерируем requestId один раз на inviteCode
        if (!requestIdRef.current) {
            requestIdRef.current = uuidv4();
        }

        acceptInvite.mutate(
            { inviteCode, requestId: requestIdRef.current },
            {
                onSuccess: (response) => {
                    navigate(`/s/${response.serverId}`, { replace: true });
                },
                onError: (error) => {
                    console.error('Accept invite error:', error);
                },
            }
        );
    }, [inviteCode]);

  return null;
}

export default InviteRedirect;