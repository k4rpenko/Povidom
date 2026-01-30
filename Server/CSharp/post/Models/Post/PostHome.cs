using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using posts.Models.MessageChat;
using StackExchange.Redis;
using System.Xml.Linq;

namespace posts.Models.Post
{
    public class PostHomeComment
    {
        public string Id { get; set; }
        public string? UserId { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<string>? Likes { get; set; } = new List<string>();
        public int? LikeAmount { get; set; }
        public bool? YouLike { get; set; }
        public UserFind User { get; set; }
    }
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
        public List<PostHomeComment>? Comments { get; set; } = new List<PostHomeComment>();
        public bool? YouComment { get; set; }
        public int? CommentAmount { get; set; }
        public List<UserFind>? Views { get; set; } = new List<UserFind>();
        public int ViewsAmount { get; set; }
        public bool YouView { get; set; }
        public bool? SPublished { get; set; }
        public bool? ShaveAnswer { get; set; }
        public string? Ansver { get; set; }

    }
}
