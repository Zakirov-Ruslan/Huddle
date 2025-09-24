import { useEffect, useRef } from "react";
import { useParams, useNavigate } from "react-router";
import { useAcceptInvite } from "../hooks/invitesApiHooks";

function InviteRedirect() {
  const { inviteCode } = useParams();
  const navigate = useNavigate();
  const acceptInvite = useAcceptInvite();

  const invokedRef = useRef(false);

  useEffect(() => {
    if (!inviteCode) return;
    if (invokedRef.current) return;
    invokedRef.current = true;

    acceptInvite.mutate(inviteCode, {
      onSuccess: (response) => {
        navigate(`/s/${response.serverId}`, { replace: true });
      },
       onError: (error) => {
           console.log(error);
       },
    });
  }, [inviteCode]);

  return null;
}

export default InviteRedirect;