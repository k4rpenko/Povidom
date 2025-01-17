using UserServer.Models.MessageChat;
using UserServer.Models.Post;

namespace UserServer.Models.Users
{
    public class UserAccount
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public string? UserName { get; set; }
        public string? Title { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Заміна полів на властивості
        public List<PostHome> Post { get; set; } = new List<PostHome>();
        public List<PostHome> RecallPost { get; set; } = new List<PostHome>();

        public int FollowersAmount { get; set; }
        public List<AccountSettingsModel> Followers { get; set; } = new List<AccountSettingsModel>();
        public bool YouFollower { get; set; }
        public int SubscribersAmount { get; set; }
        public List<AccountSettingsModel> Subscribers { get; set; } = new List<AccountSettingsModel>();
        public bool YouSubscriber { get; set; }
    }
}
