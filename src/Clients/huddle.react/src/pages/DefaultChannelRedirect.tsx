import { Navigate, useParams } from "react-router";
import { useServer } from "../hooks/serverApiHooks";

export default function DefaultChannelRedirect() {
  const { serverId } = useParams();
  if (!serverId) return <div>Invalid server ID</div>;

  const { data: server, isPending, error } = useServer(serverId);

  if (isPending) return null; // maybe add skeleton here later
  if (error) return <div>Failed to load server</div>;
  if (!server) return null;

  const textChannel = server.channels.find(ch => ch.channelType.toLowerCase() === "text");
  const firstAnyChannel = server.channels[0];

  const targetChannel = textChannel ?? firstAnyChannel;
  if (!targetChannel) {
    return <div>No channels yet</div>;
  }

  return <Navigate to={`ch/${targetChannel.id}`} replace />;
}