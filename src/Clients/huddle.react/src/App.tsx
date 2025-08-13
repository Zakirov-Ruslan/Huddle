import { QueryClientProvider, QueryClient } from '@tanstack/react-query'
import './App.css'
import { BrowserRouter, Route, Routes } from "react-router"
import Layout from './pages/Layout'
import Server from './pages/Server'
import Home from './pages/Home'
import { Navigate } from 'react-router'
import { AuthProvider } from "react-oidc-context";
import type { User } from 'oidc-client-ts'
import { Channel } from './components/Channel'
import InviteRedirect from './pages/InviteRedirect'
import Root from './pages/Root'

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

    return (
        <>
            <AuthProvider {...config} onSigninCallback={onSigninCallback}>
                <QueryClientProvider client={queryClient}>
                    <BrowserRouter>
                        <Routes>
                            <Route element={<Root />} >
                                <Route path="/" element={<Layout />}>
                                    <Route index element={<Navigate to="/h" replace />} />
                                    <Route path='h' element={<Home />} />
                                    <Route path="s/:serverId" element={<Server />} >
                                        <Route path="ch/:channelId" element={<Channel />} />
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
