import { createSignalRContext } from "react-signalr";
import { useAuth } from "react-oidc-context";
import { GATEWAY_URL } from "../api/api";
import { Outlet } from "react-router";

export const SignalRContext = createSignalRContext();

export default function SignalRProvider() {
    const auth = useAuth();

    return (
        <SignalRContext.Provider
            connectEnabled={!!auth.user?.access_token}
            accessTokenFactory={() => auth.user!.access_token}
            dependencies={[auth.user?.access_token]}
            url={`${GATEWAY_URL}/notifications/hub`}
        >
            <Outlet />
        </SignalRContext.Provider>
    );
}


