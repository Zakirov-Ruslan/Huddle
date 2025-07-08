public class VoiceHub : Hub
{
    public async Task JoinRoom(string roomId, string username)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("UserJoined", new { ConnectionId = Context.ConnectionId, Username = username });
    }

    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("UserLeft", Context.ConnectionId);
    }

    public async Task SendAudioStream(string roomId, string userId, byte[] audioData)
    {
        await Clients.Group(roomId).SendAsync("ReceiveAudioStream", userId, audioData);
    }
}