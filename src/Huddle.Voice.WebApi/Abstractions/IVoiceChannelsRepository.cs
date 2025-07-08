using Huddle.Voice.WebApi.Models;

namespace Huddle.Voice.WebApi.Abstractions
{
    public interface IVoiceChannelsRepository
    {
        Task<VoiceUser> GetUserAsync(Guid channelId, string connectionId);
        Task<bool> AddUserToRoomAsync(Guid channelId, VoiceUser user);
        Task<bool> RemoveUserFromRoomAsync(Guid channelId, string connectionId);
    }
}