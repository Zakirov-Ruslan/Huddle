import { useQuery } from "@tanstack/react-query";
import { getUserProfile, getMyProfile } from "../../api/users/usersApi";
import type { UserProfileDto } from "../../api/types";

export const useUserProfile = (userId: string) => {
    return useQuery<UserProfileDto, Error>({
        queryKey: ['profile', userId], 
        queryFn: () => getUserProfile(userId),
        enabled: !!userId,
        staleTime: 2 * 60 * 1000,
    });
};

export const useMyProfile = () => {
    return useQuery<UserProfileDto, Error>({
        queryKey: ['profile', 'me'],
        queryFn: getMyProfile,
        staleTime: 30 * 60 * 1000,
    });
};