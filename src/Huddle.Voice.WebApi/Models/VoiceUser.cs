namespace Huddle.Voice.WebApi.Models
{
    public class VoiceUser
    {
        public Guid UserId { get; init; }
        public string Username { get; init; }
        public string ConnectionId { get; init; }
        public bool IsMuted { get; set; }
        public bool IsCameraOn { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
