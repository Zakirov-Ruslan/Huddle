//import { useEffect, useState } from "react";
//import { useAuth } from "react-oidc-context";
//import { Outlet } from "react-router";
//import { useNavigate } from "react-router";
//import { createSignalRContext } from "react-signalr";
//import { GATEWAY_URL } from "../api/api";
//import { LiveKitRoom, RoomContext } from "@livekit/components-react";
//import { Room } from "livekit-client";

//export const SignalRContext = createSignalRContext();
//function Root() {
//    const auth = useAuth();
//    const navigate = useNavigate();

//    //live kit
//    const [room] = useState(() => new Room({}));

//    // You can manage room connection lifecycle here
//    useEffect(() => {
//        //room.connect('your-server-url', 'your-token');
//        //return () => {
//        //    room.disconnect();
//        //};
//        console.log(room);
//    }, [room]);
//    //const [token, setToken] = useState<string | undefined>(undefined);

//        return (
//            <SignalRContext.Provider
//                connectEnabled={!!auth.user?.access_token}
//                accessTokenFactory={() => auth.user!.access_token}
//                dependencies={[auth.user?.access_token]} //remove previous connection and create a new connection if changed
//                url={`${GATEWAY_URL}/notifications/hub`}
//            >
//                <RoomContext.Provider
//                    value={ room }
//                >
//                    < Outlet />
//                </RoomContext.Provider>

//            </SignalRContext.Provider >
//        )



//}

//export default Root;