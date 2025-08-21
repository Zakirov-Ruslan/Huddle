using Huddle.Grpc;

namespace Huddle.SignalR.Service
{
    public class GrpcChannelAccessClient
    {
        private readonly ChannelAccess.ChannelAccessClient _client;

        public GrpcChannelAccessClient(ChannelAccess.ChannelAccessClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<bool> CheckChannelAccessAsync(Guid channelId, Guid userId)
        {
            var response = await _client.CheckChannelAccessAsync(new ChannelAccessRequest
            {
                ChannelId = channelId.ToString(),
                IdentityId = userId.ToString()
            });

            return response.HasAccess;
        }

        public async Task<bool> CheckServerAccessAsync(Guid serverId, Guid userId)
        {

            var response = await _client.CheckServerAccessAsync(new ServerAccessRequest
            {
                ServerId = serverId.ToString(),
                IdentityId = userId.ToString()
            });

            return response.HasAccess;
        }
    }
}
