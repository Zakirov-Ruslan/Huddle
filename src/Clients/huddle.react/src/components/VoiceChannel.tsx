import type { ChannelDto } from "../api/dtos"
import { LiveKitRoom, useLiveKitRoom } from "@livekit/components-react"
import { useLiveKitToken } from "../hooks/voiceApiHooks";
import { useEffect } from "react";

const VoiceChannel: React.FC<ChannelDto> = ({ id, serverId, name, channelType }) => {

    const { data, error, isLoading } = useLiveKitToken(serverId, id);

    useEffect(() => {
        if (data) {
            console.log(data);
        }
    }, [data])

    return (

        <LiveKitRoom token={data} serverUrl="" connect={true}>
            <p>{ name }</p>
        </LiveKitRoom>
    );
}

export default VoiceChannel;