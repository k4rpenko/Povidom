namespace Server.Interface.Sending
{
    public interface IEmailSeding
    {
        public Task PasswordCheckEmailAsync(string EmailTo, string url, string scheme, string host);
        public Task Writing(string EmailTo, string text);
    }
}
