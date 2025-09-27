import { useEffect, useRef } from "react";
import { useParams, useNavigate } from "react-router";
import { useAcceptInvite } from "../hooks/invitesApiHooks";

function InviteRedirect() {
  const { inviteCode } = useParams();
  const navigate = useNavigate();
  const acceptInvite = useAcceptInvite();

  //const invokedRef = useRef(false);
  const isProcessingRef = useRef(false);

  useEffect(() => {
      if (!inviteCode) return;
      if  (isProcessingRef.current || acceptInvite.isPending) return;
      
      isProcessingRef.current = true;

      acceptInvite.mutate(inviteCode, {
          onSuccess: (response) => {
              navigate(`/s/${response.serverId}`, { replace: true });
          },
          onError: (error) => {
              console.log(error);
              // Сбрасываем флаги при ошибке, чтобы можно было повторить попытку
              isProcessingRef.current = false;
          },
          onSettled: () => {
              // Сбрасываем флаг обработки после завершения запроса
              isProcessingRef.current = false;
          }
      });
  }, [inviteCode, acceptInvite, navigate]);

  return null;
}

export default InviteRedirect;