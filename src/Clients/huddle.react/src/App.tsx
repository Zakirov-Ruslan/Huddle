import { QueryClientProvider, QueryClient } from '@tanstack/react-query'
import './App.css'
import { BrowserRouter, Route, Routes } from "react-router"
import Layout from './pages/Layout'
import Server from './pages/Server'
import Home from './pages/Home'
import { Navigate } from 'react-router'
import { AuthProvider } from "react-oidc-context";
import InviteRedirect from './pages/InviteRedirect'
import SignalRProvider from './providers/SignalRProvider'
import LiveKitProvider from './providers/LiveKitProvider'
import DefaultChannelRedirect from './pages/Channel/DefaultChannelRedirect'
import RequireAuth from './pages/RequireAuth'
import ChannelSettings from './pages/ChannelSettings'
import UserSettings from './pages/UserSettings'
import ServerProfile from './pages/ServerSettings/ServerProfile'
import ServerSettings from './pages/ServerSettings/ServerSettings'
import { Channel } from './pages/Channel/Channel'
import SignalRHandlersWrapper from './providers/SignalRHandlersWrapper'
import { useEffect } from 'react'
import { initSessionId } from './utils/authHelpers'

const queryClient = new QueryClient()

const identityUrl = import.meta.env.VITE_IDENTITY_URL;

const config = {
    authority: identityUrl,
    client_id: 'interactive.confidential',
    redirect_uri: `${window.location.origin}/callback`,
    response_type: 'code',
    scope: 'openid profile api1 offline_access',
    post_logout_redirect_uri: window.location.origin,
};


function App() {

    const onSigninCallback = () => {
        const defaultPath = '/';
        const path = sessionStorage.getItem('preAuthPath') || defaultPath;
        window.history.replaceState({}, document.title, path);
        window.location.href = path;
    };

    useEffect(() => {
        initSessionId();
    }, []);

    return (
        <>
            <AuthProvider {...config} onSigninCallback={onSigninCallback}>
                <QueryClientProvider client={queryClient}>
                    <BrowserRouter>
                        <Routes>
                            <Route element={<RequireAuth />} >
                                <Route element={<SignalRProvider />}>
                                    <Route element={<SignalRHandlersWrapper/>} >
                                        <Route element={<LiveKitProvider/>}>
                                            <Route path="/" element={<Layout />}>
                                                <Route index element={<Navigate to="/h" replace />} />
                                                <Route path='h' element={<Home />} />
                                                <Route path="s/:serverId" element={<Server />} >
                                                    <Route index element={<DefaultChannelRedirect />} />
                                                    <Route path="ch/:channelId" element={<Channel />} />
                                                </Route>
                                            </Route>
                                            <Route path='server-settings/:serverId' element={<ServerSettings />}>
                                                <Route index element={<Navigate to="server-profile" replace />} />
                                                <Route path='server-profile' element={<ServerProfile/>} />
                                            </Route>
                                            <Route path='channel-settings/:channelId' element={<ChannelSettings />} />
                                            <Route path='user-settings/:userId' element={<UserSettings />} />
                                        </Route>
                                    </Route>
                                </Route>
                                <Route path="invite/:inviteCode" element={<InviteRedirect />} />
                            </Route>
                        </Routes>
                    </BrowserRouter>
                </QueryClientProvider>
            </AuthProvider>
        </>
    )
}

export default App
