import { useEffect } from "react";
import { useAuth } from "react-oidc-context";
import { Outlet } from "react-router";
import { useNavigate } from "react-router";

function Root() {
    const auth = useAuth();
    const navigate = useNavigate();

    useEffect(() => {
        if (!auth.isLoading && !auth.isAuthenticated && !auth.error) {
            sessionStorage.setItem('preAuthPath', window.location.pathname);
            auth.signinRedirect();
        }
    }, [auth, auth.isLoading, auth.isAuthenticated, auth.error]);

    useEffect(() => {
        if (auth.isAuthenticated) {
            const preAuthPath = sessionStorage.getItem('preAuthPath');
            if (preAuthPath && preAuthPath !== '/callback') {
                sessionStorage.removeItem('preAuthPath');
                navigate(preAuthPath);
            }
        }
    }, [auth.isAuthenticated, navigate]);

    switch (auth.activeNavigator) {
        case "signinSilent":
            return <div>Signing you in...</div>;
        case "signoutRedirect":
            return <div>Signing you out...</div>;
    }

    if (auth.isLoading) {
        return <div>Loading...</div>;
    }

    if (auth.error) {
        return <div>Oops... {auth.error.message}</div>;
    }

    if (auth.isAuthenticated) {
        return <Outlet />;
    }

    return <div>Redirecting to login...</div>;
}

export default Root;