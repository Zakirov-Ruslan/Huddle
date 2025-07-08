namespace Huddle.Voice.WebApi.Models
{
    public class VoiceRoom
    {
        public Guid Id { get; set; }
        public List<VoiceUser> Users { get; set; } = [];
    }
}
