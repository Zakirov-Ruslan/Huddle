import TextChannel from "./TextChannel";
import VoiceChannel from "./VoiceChannel";
import { useParams } from 'react-router';
import { useServerContext } from "../pages/Server";

export const Channel = () => {
    const { channelId } = useParams<{ channelId: string }>();

    const serverContext = useServerContext();
    if (!serverContext || !serverContext.server)
        return;

    const channel = serverContext.server.channels.find(ch => ch.id === channelId);
    if (!channel)
        return;

    return (
        <div className="flex min-h-0 flex-row" >

            {channel.channelType.toLowerCase() === "text" ? (
                <TextChannel {...channel} />
            ) : channel.channelType.toLowerCase() === "voice" ? (
                <VoiceChannel {...channel} />
            ) : (
                <div>Unsupported channel type</div>
            )}


            {serverContext.isShowMembers && (
                (<div className="w-60 border-l-1 border-gray-200 bg-white"> </div>)
            )}
        </div>
    );

};