namespace Huddle.Channel.Application.Services
{
    public interface IAccessService
    {
        Task<bool> CanUserAccessChannelAsync(Guid channelId, Guid identityId);
        Task<bool> IsUserMemberOfServerAsync(Guid serverId, Guid userId);
    }
}
