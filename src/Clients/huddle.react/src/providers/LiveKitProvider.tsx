import { useEffect, useState } from "react";
import { LiveKitRoom, RoomContext } from "@livekit/components-react";
import { Room } from "livekit-client";
import { Outlet } from "react-router";

export default function LiveKitProvider() {
    const [room] = useState(() => new Room({}));

    useEffect(() => {
        // room.connect(url, token)
        // return () => room.disconnect();
    }, [room]);

    return (
        <RoomContext.Provider value={room}>
             <Outlet />
        </RoomContext.Provider>
    );
}


