namespace AuthServer.Models.Users
{
    public class AccountSettingsModel
    {
        public string? Token { get; set; }
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? OriginPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? Password { get; set; }
        public string? Avatar { get; set; }
        public string? NickName { get; set; }
        public string? Title { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Appeal { get; set; }
    }
}
