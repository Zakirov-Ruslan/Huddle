using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Huddle.SignalR
{
    [Authorize]
    public class NotificationsHub : Hub
    {
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
            await Groups.AddToGroupAsync(Context.ConnectionId, $"server:{serverId}");
        }

        public async Task LeaveServer(string serverId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"server:{serverId}");
        }

        public async Task JoinChannel(string channelId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"channel:{channelId}");
        }

        public async Task LeaveChannel(string channelId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"channel:{channelId}");
        }
    }
}