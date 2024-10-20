using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Server.Models
{
    public class Comment
    {
        [BsonElement("AuthorId")]
        public string AuthorId { get; set; }

        [BsonElement("Content")]
        public string Content { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } 
    }

    public class SpacePostModel
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; }

        [BsonElement("Content")]
        public string Content { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [BsonElement("MediaUrls")]
        public List<string>? MediaUrls { get; set; }

        [BsonElement("LikeCount")]
        public List<string>? LikeCount { get; set; }

        [BsonElement("RetweetCount")]
        public List<string>? RetweetCount { get; set; }

        [BsonElement("Hashtags")]
        public List<string>? Hashtags { get; set; }

        [BsonElement("Mentions")]
        public List<string>? Mentions { get; set; }

        [BsonElement("Comments")]
        public List<Comment>? Comments { get; set; }

        [BsonElement("SPublished")]
        public bool SPublished { get; set; }
    }
}
