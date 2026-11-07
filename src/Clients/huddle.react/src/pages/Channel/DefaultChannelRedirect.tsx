import { Navigate, useParams } from "react-router";
import { useServer } from "../../hooks/queries/servers";
import { useIsMobile } from "../../hooks/useIsMobile";

/**
 * Index route of a server (`/s/:serverId`).
 *
 * On desktop we want a channel to be opened right away, so we redirect to the
 * first text channel. On phones this very URL *is* the "channel list" screen,
 * so redirecting would immediately hide the list — there we render nothing and
 * let `Server` show the channel sidebar.
 */
export default function DefaultChannelRedirect() {
  const { serverId } = useParams();
  const isMobile = useIsMobile();
  const { data: server, error } = useServer(serverId);

  if (!serverId) return <div className="flex flex-1 items-center justify-center">Invalid server ID</div>;
  if (error) return <div className="flex flex-1 items-center justify-center">Failed to load server</div>;
  if (isMobile) return null;
  if (!server) return null;

  const textChannel = server.channels.find(ch => ch.channelType.toLowerCase() === "text");
  const firstAnyChannel = server.channels[0];

  const targetChannel = textChannel ?? firstAnyChannel;
  if (!targetChannel) {
    return <div>No channels yet</div>;
  }

  return <Navigate to={`ch/${targetChannel.id}`} replace />;
}