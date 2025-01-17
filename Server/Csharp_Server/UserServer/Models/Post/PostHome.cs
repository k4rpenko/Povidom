using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using UserServer.Models.MessageChat;
using StackExchange.Redis;
using System.Xml.Linq;

namespace UserServer.Models.Post
{
    public class PostHome
    {
        public string Id { get; set; }
        public UserFind User { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<string>? MediaUrls { get; set; }
        public List<Like>? LikeArray { get; set; } = new List<Like>();
        public bool? YouLike { get; set; }
        public int? LikeAmount { get; set; }
        public int? Retpost { get; set; }
        public bool? YouRetpost { get; set; }
        public int? RetpostAmount { get; set; }
        public int? Hashtags { get; set; }
        public int? Mentions { get; set; }
        public List<UserFind>? Recall { get; set; } = new List<UserFind>();
        public List<Comment>? CommentsArray { get; set; } = new List<Comment>();
        public bool? YouComment { get; set; }
        public int? CommentAmount { get; set; }
        public int? Views { get; set; }
        public bool? SPublished { get; set; }
    }
}
