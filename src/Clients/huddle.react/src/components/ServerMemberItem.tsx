import { useUserProfile } from "../hooks/usersApiHooks";

const ServerMemberItem = ({ userId }: { userId: string }) => {

    const { data: profile, isLoading, error } = useUserProfile(userId);

    return (
        <div className="text-md flex flex-row items-center gap-2 rounded-md p-1.5 font-semibold text-gray-600 select-none hover:bg-gray-100">
            <div className="flex h-8 w-8 items-center justify-center rounded-full bg-[#5D6D7B] text-sm font-medium text-white">
                {profile?.userName[0]}
            </div>
            <span>{profile?.userName}</span>
        </div>
    );
}

export default ServerMemberItem;