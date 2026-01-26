import { useQueryClient } from '@tanstack/react-query';
import { useEffect } from 'react';
import { SignalRContext } from '../providers/SignalRProvider';
import { handleCreateMessage } from '../signalRHandlers/messageHandlers';

export const useGlobalSignalRHandlers = () => {
    const queryClient = useQueryClient();

    useEffect(() => {
        const connection = SignalRContext.connection;

        if (!connection || connection.state !== 'Connected') 
            return;
        
        const createMessageHandler = handleCreateMessage(queryClient);
        connection.on('CreateMessage', createMessageHandler);

        console.log('subscribed to all signalRHandlers')

        return () => {
            connection.off('CreateMessage', createMessageHandler);
        };
    }, [SignalRContext.connection?.state, queryClient]);
};
