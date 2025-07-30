import { QueryClientProvider, QueryClient } from '@tanstack/react-query'
import './App.css'
import { BrowserRouter, Route, Routes } from "react-router"
import Layout from './pages/Layout'
import Server from './pages/Server'
import Home from './pages/Home'
import Channel from './components/Channel'
import { Navigate } from 'react-router'
import { AuthProvider } from "react-oidc-context";
import type { User } from 'oidc-client-ts'

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

    const onSigninCallback = (_user: User | void): void => {
        window.history.replaceState({}, document.title, window.location.pathname)
        window.location.href = '/';
    }

    return (
        <>
            <AuthProvider {...config} onSigninCallback={onSigninCallback}>
                <QueryClientProvider client={queryClient}>
                    <BrowserRouter>
                        <Routes>
                            <Route path="/" element={<Layout />}>
                                <Route index element={<Navigate to="/h" replace />} />
                                <Route path='h' element={<Home />} />
                                <Route path="s/:serverId" element={<Server />} >
                                    <Route path="ch" element={<Channel />} />
                                </Route>
                            </Route>
                        </Routes>
                    </BrowserRouter>
                </QueryClientProvider>
            </AuthProvider>
        </>
    )
}

export default App
