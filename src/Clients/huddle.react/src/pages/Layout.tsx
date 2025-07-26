import { Link, Outlet } from "react-router";
import { GoHomeFill } from "react-icons/go";
import { useAuth } from "react-oidc-context";
import { FaMicrophone, FaMicrophoneSlash } from "react-icons/fa";
import { TbHeadphonesOff } from "react-icons/tb";
import { useMyServers } from "../api/servers/serverApiHooks";

function Layout() {

    const auth = useAuth();

    const { data: servers, error, isPending } = useMyServers();

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
        return <div>Oops... {auth.error.name} caused {auth.error.message}</div>;
    }

    if (auth.isAuthenticated) {

        //const { data: servers, error, isPending } = useMyServers();

        return (
            <>
                <div className="grid h-full grid-cols-[auto_1fr]">
                    <div>
                        <div className="">
                            <div className="flex flex-col items-center gap-3 p-4">
                                <Link to="/h" className="align-center flex h-15 w-15 items-center justify-center rounded-xl bg-slate-200">
                                    <GoHomeFill className="h-8 w-8" />
                                </Link>
                                <div className="my-1 w-13 border-b border-slate-300"></div>
                                <Link to="s" className="align-center flex h-15 w-15 items-center justify-center rounded-xl bg-slate-200">
                                    C
                                </Link>
                                <Link to="s" className="align-center flex h-15 w-15 items-center justify-center rounded-xl bg-slate-200">
                                    C
                                </Link>
                                {
                                    servers?.map(server => 
                                    <Link to="s" className="align-center flex h-15 w-15 items-center justify-center rounded-xl bg-slate-200">
                                        { server.name }
                                    </Link>
                                )}
                            </div>
                        </div>
                    </div>
                    <div className="flex flex-row pt-4">
                        <div className="flex-grow-1 overflow-hidden rounded-tl-2xl bg-slate-100">
                            <Outlet />
                        </div>
                    </div>
                </div>
                <div className="absolute bottom-2 left-2 z-10 flex h-15 w-83 flex-row gap-2 rounded-md bg-slate-300 p-2">
                    <div className="flex-grow-1">
                        {auth.user?.profile.name ?? 'username must be here'}
                    </div>
                    <button className="">
                        <FaMicrophone /> 
                    </button>
                    <button className="">
                        <TbHeadphonesOff />
                    </button>
                </div>
            </>
        );
    }

    auth.signinRedirect();

}

export default Layout;