import { useAuth } from "react-oidc-context";
import { Outlet } from "react-router";
import Mascot from "../components/Mascot";
import { AppShellSkeleton } from "../components/skeleton/Skeletons";

export default function RequireAuth() {
    const auth = useAuth();

    if (auth.isLoading)
        return <AppShellSkeleton />;

    if (auth.error)
        return (
            <div className="flex h-full flex-col items-center justify-center gap-3 bg-white p-6 text-center">
                <Mascot pose="sad" alt="Sad Huddle mascot" className="h-40" />
                <span className="text-lg font-medium text-gray-700">We could not sign you in</span>
                <span className="max-w-md text-sm text-gray-500">{auth.error.message}</span>
                <button
                    type="button"
                    onClick={() => void auth.signinRedirect()}
                    className="rounded-md bg-[#5D6D7B] px-4 py-2 font-medium text-white transition-colors duration-150 hover:bg-[#4F5E6A]"
                >
                    Try again
                </button>
            </div>
        );

    if (!auth.isAuthenticated) {
        sessionStorage.setItem('preAuthPath', window.location.pathname);
        auth.signinRedirect();
        return <AppShellSkeleton />;
    }

    return <Outlet />;
}