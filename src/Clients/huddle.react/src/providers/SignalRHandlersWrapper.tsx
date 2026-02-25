import { Outlet } from "react-router";
import { useGlobalSignalRHandlers } from "../hooks/useGlobalSignalRHandlers";

export default function SignalRHandlersWrapper() {
    useGlobalSignalRHandlers();

    return <Outlet />;
}