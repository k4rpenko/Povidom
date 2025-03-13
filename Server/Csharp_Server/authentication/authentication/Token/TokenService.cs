namespace authentication.Token
{
    public class TokenService
    {
        public string GetToken(string AccessToken)
        {
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            return token;
        }
    }
}
