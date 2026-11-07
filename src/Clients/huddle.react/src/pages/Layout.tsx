import { Outlet } from "react-router";
import ServerRail from "../components/layout/ServerRail";
import UserPanel from "../components/layout/UserPanel";

/**
 * Application shell.
 *
 *   [ server rail ][ main area ]
 *
 * The desktop keeps the floating user card at the bottom-left of the content
 * area; on phones/tablets the same component is docked to the bottom of the
 * channel list (`ServerSidebar`) instead, so it never covers the chat.
 */
function Layout() {
    return (
        <div className="relative flex h-full w-full overflow-hidden bg-gray-100">
            <ServerRail />

            <main className="flex min-h-0 min-w-0 flex-1 flex-col overflow-hidden md:pt-4">
                <div className="flex min-h-0 flex-1 flex-col overflow-hidden bg-gray-100 md:rounded-tl-2xl md:border md:border-r-0 md:border-b-0 md:border-gray-300">
                    <Outlet />
                </div>
            </main>

            {/* Desktop only: same card, floating over the bottom-left of the content area. */}
            <div className="pointer-events-none absolute bottom-4 left-4 z-30 hidden w-90 lg:block">
                <UserPanel className="pointer-events-auto" />
            </div>
        </div>
    );
}

export default Layout;

