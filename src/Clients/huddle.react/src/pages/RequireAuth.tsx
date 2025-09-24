import { useAuth } from "react-oidc-context";
import { Outlet } from "react-router";

export default function RequireAuth() {
    const auth = useAuth();

    if (auth.isLoading)
        return <div>Loading...</div>;

    if (auth.error)
        return <div>Oops... {auth.error.message}</div>;

    if (!auth.isAuthenticated) {
        sessionStorage.setItem('preAuthPath', window.location.pathname);
        auth.signinRedirect();
        return <div>Redirecting to login...</div>;
    }

    return <Outlet />;
}