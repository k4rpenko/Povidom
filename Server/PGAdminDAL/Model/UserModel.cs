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
        public Dictionary<string, string> LikePost { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> CommentPost { get; set; } = new Dictionary<string, string>();
    }
}
