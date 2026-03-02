import { useQueryClient } from '@tanstack/react-query';
import { useEffect } from 'react';
import { SignalRContext } from '../providers/SignalRProvider';
import { handleCreateMessage } from '../signalRHandlers/messages';
import { handleCreateChannel } from '../signalRHandlers/channels';
import { getSessionId } from '../utils/authHelpers';

export const useGlobalSignalRHandlers = () => {
    const queryClient = useQueryClient();

    useEffect(() => {
        const connection = SignalRContext.connection;
        if (!connection) 
            return;
        
        const createMessageHandler = withSessionFilter(handleCreateMessage(queryClient));
        connection.on('CreateMessage', createMessageHandler);

        const createChannelHandler = handleCreateChannel(queryClient);
        connection.on('CreateChannel', createChannelHandler);

        console.debug('subscribed to all signalRHandlers')

        return () => {
            connection.off('CreateMessage', createMessageHandler);
            connection.off('CreateChannel', createChannelHandler);
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