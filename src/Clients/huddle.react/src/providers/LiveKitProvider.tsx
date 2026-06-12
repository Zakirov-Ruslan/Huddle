import { LiveKitRoom, RoomAudioRenderer } from "@livekit/components-react";
import { useVoiceChannelStore } from "../stores/voiceChannelStore";
import { Outlet } from "react-router";

export default function LiveKitProvider() {
    const { activeChannel, token } = useVoiceChannelStore();

    return (
        <LiveKitRoom
            className="h-screen w-screen"
            serverUrl="wss://localhost:7062/liveKit-server/"
            token={token || ""} // Пустая строка, если токена нет
            connect={!!(activeChannel && token)} // Подключаемся только если есть канал и токен
            audio={true}
            video={false}
            onDisconnected={() => {
                useVoiceChannelStore.getState().leaveChannel();
            }}
        >
            <RoomAudioRenderer />
            <Outlet />
        </LiveKitRoom>
    );
}