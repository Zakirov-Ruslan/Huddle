import { useQueryClient } from '@tanstack/react-query';
import { useEffect } from 'react';
import { SignalRContext } from '../providers/SignalRProvider';
import { handleCreateMessage } from '../signalRHandlers/messages';
import { handleCreateChannel } from '../signalRHandlers/channels';
import { handleVoiceChannelClosed, handleVoiceParticipantJoined, handleVoiceParticipantLeft } from '../signalRHandlers/voice';
import { getSessionId } from '../utils/authHelpers';

export const useGlobalSignalRHandlers = () => {
    const queryClient = useQueryClient();

    useEffect(() => {
        const connection = SignalRContext.connection;
        if (!connection) 
            return;
        
        const createMessageHandler = withSessionFilter(handleCreateMessage(queryClient));
        connection.on('CreateMessage', createMessageHandler);

        const createChannelHandler = withSessionFilter(handleCreateChannel(queryClient));
        connection.on('CreateChannel', createChannelHandler);

        // Voice presence is a delta against the snapshot cached by `useServerVoiceState`.
        const voiceParticipantJoinedHandler = handleVoiceParticipantJoined(queryClient);
        connection.on('VoiceParticipantJoined', voiceParticipantJoinedHandler);

        const voiceParticipantLeftHandler = handleVoiceParticipantLeft(queryClient);
        connection.on('VoiceParticipantLeft', voiceParticipantLeftHandler);

        const voiceChannelClosedHandler = handleVoiceChannelClosed(queryClient);
        connection.on('VoiceChannelClosed', voiceChannelClosedHandler);

        console.debug('subscribed to all signalRHandlers')

        return () => {
            connection.off('CreateMessage', createMessageHandler);
            connection.off('CreateChannel', createChannelHandler);
            connection.off('VoiceParticipantJoined', voiceParticipantJoinedHandler);
            connection.off('VoiceParticipantLeft', voiceParticipantLeftHandler);
            connection.off('VoiceChannelClosed', voiceChannelClosedHandler);
        };
    }, [SignalRContext.connection, queryClient]);
};

export const withSessionFilter = <T extends Record<string, any>>(
    handler: (data: T) => void
) => {
    return (data: T & { initiatorSessionId?: string }) => {
        const mySessionId = getSessionId();
        
        if (data.initiatorSessionId === mySessionId) {
            console.debug('Ignoring self initiated event:', data);
            return;
        }

        handler(data);
    };
};