using Huddle.SignalR.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Huddle.SignalR
{
    [Authorize]
    public class NotificationsHub : Hub
    {
        private readonly GrpcChannelAccessClient _grpcChannelAccessClient;

        public NotificationsHub(GrpcChannelAccessClient grpcChannelAccessClient)
        {
            _grpcChannelAccessClient = grpcChannelAccessClient;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            // No need to clear user groups - signalR GroupManager makes it automatically
            //var userId = Context.UserIdentifier;
            //await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user:{userId}");

            //await base.OnDisconnectedAsync(ex);
        }

        public async Task JoinServer(string serverId)
        {
            var isAccessible = await _grpcChannelAccessClient.CheckServerAccessAsync(Guid.Parse(serverId), Guid.Parse(Context.UserIdentifier));

            if (isAccessible)
                await Groups.AddToGroupAsync(Context.ConnectionId, $"server:{serverId}");
        }

        public async Task LeaveServer(string serverId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"server:{serverId}");
        }

        public async Task JoinChannel(string channelId)
        {
            var isAccessible = await _grpcChannelAccessClient.CheckChannelAccessAsync(Guid.Parse(channelId), Guid.Parse(Context.UserIdentifier));

            if (isAccessible)
                await Groups.AddToGroupAsync(Context.ConnectionId, $"channel:{channelId}");
        }

        public async Task LeaveChannel(string channelId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"channel:{channelId}");
        }
    }
}