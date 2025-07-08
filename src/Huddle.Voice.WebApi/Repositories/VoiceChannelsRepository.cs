using Huddle.Voice.WebApi.Abstractions;
using Huddle.Voice.WebApi.Models;
using StackExchange.Redis;

namespace Huddle.Voice.WebApi.Repositories
{
    public class VoiceChannelsRepository : IVoiceChannelsRepository
    {
        private readonly IDatabase _redis;

        public VoiceChannelsRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            _redis = connectionMultiplexer.GetDatabase();
        }
        public async Task<VoiceUser?> GetUserAsync(Guid channelId, string connectionId)
        {
            var key = $"room:{channelId}:users:{connectionId}";
            var userData = await _redis.HashGetAllAsync(key);

            if (userData.Length == 0) return null;

            return null;
            //return new VoiceUser
            //{
            //    ConnectionId = connectionId,
            //    UserId = userData["userId"],
            //    Username = userData["username"],
            //    IsMuted = userData["isMuted"] == "True",
            //    IsCameraOn = userData["isCameraOn"] == "True",
            //    JoinedAt = DateTime.Parse(userData["joinedAt"])
            //};
        }

        public async Task<List<VoiceUser>> GetRoomMembersAsync(string channelId)
        {
            var memberKeys = await _redis.SetMembersAsync($"room:{channelId}:members");
            var users = new List<VoiceUser>();

            //foreach (var key in memberKeys)
            //{
            //    var data = await _redis.HashGetAllAsync(key);
            //    if (data.Length == 0) continue;

            //    users.Add(new VoiceUser
            //    {
            //        ConnectionId = Guid.Parse(key.ToString().Split(":")[^1]),
            //        UserId = data["userId"],
            //        Username = data["username"],
            //        IsMuted = data["isMuted"] == "True",
            //        IsCameraOn = data["isCameraOn"] == "True",
            //        JoinedAt = DateTime.Parse(data["joinedAt"])
            //    });
            //}

            return users;
        }

        public async Task<bool> AddUserToRoomAsync(Guid channelId, VoiceUser user)
        {
            var key = $"room:{channelId}:users:{user.ConnectionId}";

            await _redis.HashSetAsync(key, new[]
            {
                new HashEntry("userId", user.UserId.ToString()),
                new HashEntry("username", user.Username),
                new HashEntry("isMuted", user.IsMuted.ToString()),
                new HashEntry("isCameraOn", user.IsCameraOn.ToString()),
                new HashEntry("joinedAt", user.JoinedAt.ToString())
            });

            await _redis.SetAddAsync($"room:{channelId}:members", key);
            await _redis.KeyExpireAsync(key, (DateTime?)null, ExpireWhen.HasNoExpiry);

            return true;
        }

        public async Task<bool> RemoveUserFromRoomAsync(Guid channelId, string connectionId)
        {
            var key = $"room:{channelId}:users:{connectionId}";
            var removed = await _redis.KeyDeleteAsync(key);

            await _redis.SetRemoveAsync($"room:{channelId}:members", key);

            return removed;
        }

    }
}
