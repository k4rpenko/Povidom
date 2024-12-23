using Server.Models.Post;

namespace Server.Models.Users
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
        public List<SpacePostModel> Post = new List<SpacePostModel>();
        public List<SpacePostModel> RecallPost = new List<SpacePostModel>();
        public int FollowersAmount { get; set; }
        public List<AccountSettingsModel> Followers = new List<AccountSettingsModel>();
        public bool YouFollower { get; set; }
        public int SubscribersAmount { get; set; }
        public List<AccountSettingsModel> Subscribers = new List<AccountSettingsModel>();
        public bool YouSubscriber { get; set; }
    }
}
