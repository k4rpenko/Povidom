using Microsoft.AspNetCore.Identity;

namespace PGAdminDAL.Model
{
    public class UserModel : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Avatar { get; set; }
        public string? Title { get; set; }
        public string? Location { get; set; }
        public string? PublicKey { get; set; }
        public string? PrivateKey { get; set; }
        public string? ConnectionId { get; set; }

        public List<string> StoriesId { get; set; } = new List<string>();
        public List<string> Subscribers { get; set; } = new List<string>();
        public List<string> Followers { get; set; } = new List<string>();
        public List<string> LikePostID { get; set; } = new List<string>();
        public List<string> CommentPostID { get; set; } = new List<string>();
        public List<string> RetweetPostID { get; set; } = new List<string>();
        public List<string> PostID { get; set; } = new List<string>();
        public List<string> RecallPostId { get; set; } = new List<string>();
        public List<string> ChatsID { get; set; } = new List<string>();

        public DateTime? LastLogin { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public bool IsVerified { get; set; } = false;
        public bool IsOnline { get; set; } = false;
    }
}
