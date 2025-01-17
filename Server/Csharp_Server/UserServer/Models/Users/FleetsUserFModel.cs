namespace UserServer.Models.Users
{
    public class FleetsUserFModel
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Avatar { get; set; }
        public string? Title { get; set; }

        public List<string>? Subscribers { get; set; } = new List<string>();
        public List<string>? Followers { get; set; } = new List<string>();
        public bool? SubscribersBool { get; set; }
        public bool? FollowersBool { get; set; }
        public List<string>? PostID { get; set; } = new List<string>();
    }
}
