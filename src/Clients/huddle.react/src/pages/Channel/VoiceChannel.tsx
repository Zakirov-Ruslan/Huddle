import { LiveKitRoom, ParticipantTile, useLiveKitRoom, useTracks } from "@livekit/components-react"
import { useEffect, useState } from "react";
import { GridLayout, ControlBar } from "@livekit/components-react";
import { useRoomContext } from "@livekit/components-react";
import type { Channel } from "../../api/types";
import { useLiveKitToken } from "../../hooks/queries/livekit";


const VoiceChannel: React.FC<Channel> = ({ id, serverId, name, channelType }) => {

    const { data, error, isLoading } = useLiveKitToken(serverId, id);
    const room = useRoomContext();
    const tracks = useTracks();

    useEffect(() => {
        let isMounted = true;

        const connectRoom = async () => {
            if (data && isMounted) {
                try {
                    if (room.state == 'connected') {
                        await room.disconnect();
                    }
                    await room.connect(`wss://localhost:7062/liveKit-server/`, data);
                } catch (err) {
                    console.error('Failed to connect to room:', err);
                }
            }
        };

        connectRoom();

        // ������� ��� ��������������� ����������
        return () => {
            isMounted = false;
            // �� ����������� �����, ��� ��� ������� ����� �������������� ������� ������������
        };
    }, [data]);

    //useEffect(() => {
    //    let start = async () => {
    //        await room.startAudio();
    //    };
    //    start();
    //}, []);

    useEffect(() => {
        if (data) {
            console.log(data);
        }
    }, [data])

    return (
        <GridLayout tracks={tracks}>
            {tracks.map((track) => (
                <ParticipantTile key={track.participant.identity} trackRef={ track} />
            ))}
            <ControlBar />
        </GridLayout>
    );
}

export default VoiceChannel;