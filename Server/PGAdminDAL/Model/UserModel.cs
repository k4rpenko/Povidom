using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PGAdminDAL.Model
{
    public class UserModel : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Avatar { get; set; }
        public string? Title { get; set; }

        public List<string> Subscribers { get; set; } = new List<string>();
        public List<string> Followers { get; set; } = new List<string>();
        public List<string> LikePostID { get; set; } = new List< string>();
        public List<string> CommentPostID { get; set; } = new List<string>();
        public List<string> RetweetPostID { get; set; } = new List<string>();
        public List<string> PostID { get; set; } = new List<string>();
    }
}
