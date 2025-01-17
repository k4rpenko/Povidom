namespace Messages.Models.MessageChat
{
    public class UserFind
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Avatar { get; set; }
        public string? Title { get; set; }
        public string? PublicKey { get; set; }
        public bool? IsOnline { get; set; }
    }
}
