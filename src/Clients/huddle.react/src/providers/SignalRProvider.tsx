import { createSignalRContext } from "react-signalr";
import { useAuth } from "react-oidc-context";
import { GATEWAY_URL } from "../api/api";
import { Outlet } from "react-router";
import { createContext, useContext, useMemo, useState } from "react";

export const SignalRContext = createSignalRContext();
export interface SignalRProviderState {
    isConnected: boolean;
}
export const SignalRStateContext = createContext<SignalRProviderState>({ isConnected: false });
export const useSignalRState = () => useContext(SignalRStateContext);

export default function SignalRProvider() {
    const auth = useAuth();

    const [isConnected, setIsConnected] = useState(false);
    const stateValue = useMemo(() => ({ isConnected }), [isConnected]);

    return (
        <SignalRStateContext.Provider value={stateValue}>
            <SignalRContext.Provider
                connectEnabled={!!auth.user?.access_token}
                accessTokenFactory={() => auth.user!.access_token}
                dependencies={[auth.user?.access_token]}
                url={`${GATEWAY_URL}/notifications/hub`}
                onOpen={() => {
                    console.debug('SignalR connection opened');
                    setIsConnected(true);
                }}
                onReconnect={() => {
                    console.debug('SignalR reconnected');
                    setIsConnected(true);
                }}
                onClosed={() => {
                    console.debug('SignalR connection closed');
                    setIsConnected(false);
                }}
            >
                <Outlet />
            </SignalRContext.Provider>
        </SignalRStateContext.Provider>
    );
}