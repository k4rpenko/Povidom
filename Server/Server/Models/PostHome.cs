using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.Models
{
    public class PostHome
    {
        public ObjectId Id { get; set; }
        public string UserId { get; set; }
        public string UserNickname { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int MediaUrls { get; set; }
        public int? Like { get; set; }
        public int? Retpost { get; set; }
        public int? InRetpost { get; set; }
        public int? Hashtags { get; set; }
        public int? Mentions { get; set; }
        public int? Comments { get; set; }
        public int? Views { get; set; }
        public bool SPublished { get; set; }
    }
}
