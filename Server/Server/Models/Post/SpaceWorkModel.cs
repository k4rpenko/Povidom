using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Server.Models.Post
{
    public class SpaceWorkModel
    {
        public string? Id { get; set; }

        [BsonElement("UserId")]
        public string? UserId { get; set; }

        [BsonElement("UserName")]
        public string? UserName { get; set; }

        [BsonElement("UserNickname")]
        public string? UserNickname { get; set; }

        [BsonElement("UserAvatur")]
        public string? UserAvatar { get; set; }

        [BsonElement("Content")]
        public string? Content { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime? CreatedAt { get; set; }

        [BsonElement("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [BsonElement("MediaUrls")]
        public List<string>? MediaUrls { get; set; }

        [BsonElement("Hashtags")]
        public List<string>? Hashtags { get; set; }

        [BsonElement("Mentions")]
        public List<string>? Mentions { get; set; }

        [BsonElement("SPublished")]
        public bool? SPublished { get; set; }
    }
}
