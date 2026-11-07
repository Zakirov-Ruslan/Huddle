import { useEffect, useMemo, useState } from "react";
import { Outlet, useNavigate, useParams } from "react-router";
import { useAuth } from "react-oidc-context";
import ReactModal from "react-modal";
import { SignalRContext, useSignalRState } from "../providers/SignalRProvider";
import type { Channel } from "../api/types";
import CreateChannelDialog from "../components/dialogs/CreateChannelDialog";
import InviteFriendsDialog from "../components/dialogs/InviteFriendsDialog";
import ServerSidebar, { type ChannelType } from "../components/server/ServerSidebar";
import ChannelHeader from "../components/server/ChannelHeader";
import { useServer } from "../hooks/queries/servers";
import { useApplicationStore } from "../stores/applicationStore";

/**
 * Server screen: channel list + channel content.
 *
 * Breakpoints:
 *  - `< md`  → two separate screens. The channel list is shown on the index
 *              route (`/s/:serverId`), the channel itself on `/s/:serverId/ch/:channelId`.
 *              The current screen is derived from the URL, so the browser's
 *              back button / Android back gesture work out of the box.
 *  - `>= md` → classic two-column layout (channel list + chat), no switching.
 */
function Server() {
    const { serverId, channelId } = useParams();

    const { isConnected } = useSignalRState();

    const setActiveServer = useApplicationStore((state) => state.setActiveServer);
    const isMembersPanelOpened = useApplicationStore((state) => state.isMembersPanelOpened);
    const toggleMembersPanelOpened = useApplicationStore((state) => state.toggleMembersPanelOpened);

    const [isCreateChannelModalOpen, setIsCreateChannelModalOpen] = useState(false);
    const [initialChannelType, setInitialChannelType] = useState<ChannelType>("text");
    const [isInviteModalOpen, setIsInviteModalOpen] = useState(false);

    const navigate = useNavigate();
    const auth = useAuth();

    const { data: server, isPending } = useServer(serverId);

    const activeChannel = useMemo(() => {
        if (!server || !channelId) return null;
        return server.channels.find(ch => ch.id === channelId) ?? null;
    }, [server, channelId]);

    useEffect(() => {
        if (serverId) {
            setActiveServer(server ?? null);
        }
    }, [server, serverId, setActiveServer]);

    useEffect(() => {
        if (!serverId || !isConnected)
            return;

        const serverJoin = async () => {
            try {
                await SignalRContext.invoke("JoinServer", serverId);
                console.debug('joined to server notification group', serverId);
            } catch (error) {
                console.error('Failed to join server notification group', error);
            }
        }

        serverJoin();

    }, [serverId, isConnected])

    const isServerOwner = Boolean(server && auth.user && server.ownerIdentityId === auth.user.profile.sub);

    /** On phones the presence of `channelId` *is* the "chat screen is open" flag. */
    const isChannelOpen = Boolean(channelId);

    const openCreateChannelDialog = (channelType: ChannelType) => {
        setInitialChannelType(channelType);
        setIsCreateChannelModalOpen(true);
    };

    const handleChannelCreated = (createdChannel: Channel | null) => {
        if (createdChannel == null)
            return;

        setIsCreateChannelModalOpen(false);
        navigate(`ch/${createdChannel.id}`);
    }

    if (!serverId)
        return <div className="flex flex-1 items-center justify-center">Invalid server ID</div>;

    return (
        <>
            <div className="flex min-h-0 flex-1 overflow-hidden">
                <ServerSidebar
                    server={server}
                    serverId={serverId}
                    isPending={isPending}
                    isOwner={isServerOwner}
                    activeChannelId={channelId}
                    className={isChannelOpen ? "max-md:hidden" : ""}
                    onCreateChannel={openCreateChannelDialog}
                    onInviteFriends={() => setIsInviteModalOpen(true)}
                />

                <section
                    className={`flex min-h-0 min-w-0 flex-1 flex-col bg-white ${isChannelOpen ? "" : "max-md:hidden"}`}
                >
                    <ChannelHeader
                        channel={activeChannel}
                        isMembersPanelOpened={isMembersPanelOpened}
                        onToggleMembers={toggleMembersPanelOpened}
                        onBack={() => navigate(`/s/${serverId}`)}
                    />

                    <div className="flex min-h-0 flex-1 flex-col overflow-hidden">
                        <Outlet key={channelId} />
                    </div>
                </section>
            </div>

            <ReactModal
                isOpen={isCreateChannelModalOpen}
                className="modal"
                overlayClassName="modal-overlay"
                onRequestClose={() => setIsCreateChannelModalOpen(false)}
                closeTimeoutMS={150}
                shouldFocusAfterRender={false}
                appElement={document.getElementById('root')!}
            >
                <CreateChannelDialog
                    serverId={serverId}
                    onCreateChannel={handleChannelCreated}
                    initialChannelType={initialChannelType}
                />
            </ReactModal>

            <ReactModal
                isOpen={isInviteModalOpen}
                className="modal"
                overlayClassName="modal-overlay"
                onRequestClose={() => setIsInviteModalOpen(false)}
                closeTimeoutMS={150}
                shouldFocusAfterRender={false}
                appElement={document.getElementById('root')!}
            >
                <InviteFriendsDialog serverId={serverId} />
            </ReactModal>
        </>
    );
}

export default Server;