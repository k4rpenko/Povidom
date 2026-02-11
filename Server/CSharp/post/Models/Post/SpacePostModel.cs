using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace posts.Models.Post
{
    public class Comment
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("UserId")]
        public string? UserId { get; set; }

        [BsonElement("Content")]
        public string? Content { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime? CreatedAt { get; set; }

        [BsonElement("Like")]
        public List<Like>? Like { get; set; } = new List<Like>();
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
        public string? UserId { get; set; }

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

        [BsonElement("Repost")]
        public List<Like>? Repost { get; set; } = new List<Like>();

        [BsonElement("IdAnswer")]
        public string? IdAnswer { get; set; }

        [BsonElement("IsAnswer")]
        public bool IsAnswer { get; set; }

        [BsonElement("Hashtags")]
        public List<string>? Hashtags { get; set; } = new List<string>();

        [BsonElement("Mentions")]
        public List<string>? Mentions { get; set; } = new List<string>();

        [BsonElement("Comments")]
        public List<Comment>? Comments { get; set; } = new List<Comment>();

        [BsonElement("Views")]
        public List<string>? Views { get; set; } = new List<string>();

        [BsonElement("SPublished")]
        public bool IsPublished { get; set; }

    }
}
