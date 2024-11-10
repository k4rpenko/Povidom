using MongoDB.Bson.Serialization.Attributes;

namespace Server.Models.MessageChat
{
    public class ChatModel
    {
        public string? IdChat { get; set; }
        public string? CreatorId { get; set; }

        public List<string>? AddUsersIdChat { get; set; }

        public string? Text { get; set; }
        public string? Img { get; set; }
        public string? Answer { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
