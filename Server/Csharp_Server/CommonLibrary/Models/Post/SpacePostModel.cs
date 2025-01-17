using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Server.Models.Post
{
    public class Comment
    {
        [BsonElement("UserId")]
        public string? UserId { get; set; }

        [BsonElement("Content")]
        public string? Content { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime? CreatedAt { get; set; }
    }

    public class Like
    {
        [BsonElement("UserId")]
        public string? UserId { get; set; }


        [BsonElement("CreatedAt")]
        public DateTime? CreatedAt { get; set; }
    }


    public class SpacePostModel
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; }

        [BsonElement("Content")]
        public string? Content { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime? CreatedAt { get; set; }

        [BsonElement("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [BsonElement("MediaUrls")]
        public List<string>? MediaUrls { get; set; } = new List<string>();

        [BsonElement("Like")]
        public List<Like>? Like { get; set; } = new List<Like>();

        [BsonElement("Retpost")]
        public List<string>? Retpost { get; set; } = new List<string>();

        [BsonElement("InRetpost")]
        public List<string>? InRetpost { get; set; } = new List<string>();

        [BsonElement("Hashtags")]
        public List<string>? Hashtags { get; set; } = new List<string>();

        [BsonElement("Mentions")]
        public List<string>? Mentions { get; set; } = new List<string>();

        [BsonElement("Recall")]
        public List<string>? Recall { get; set; } = new List<string>();

        [BsonElement("Comments")]
        public List<Comment>? Comments { get; set; } = new List<Comment>();

        [BsonElement("Views")]
        public List<string>? Views { get; set; } = new List<string>();

        [BsonElement("SPublished")]
        public bool SPublished { get; set; }

    }
}
