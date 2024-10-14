using Server.Models;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace Server.Sending
{
    public class GoogleOAuth
    {
        readonly string ClientID;
        readonly string ClientSecret;

        public GoogleOAuth()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            ClientID = builder.GetSection("GoogleAuth:ClientID").Value;
            ClientSecret = builder.GetSection("GoogleAuth:ClientSecret").Value;
        }

        public string GenerateOauthUrl(string scope, string redirectUrl, string codeChang)
        {
            var OAuthEndPoint = "ttps://accounts.google.com/o/oauth2/v2/auth";

            var queryParams = new Dictionary<string, string>
            {
                {"Client_id", ClientID},
                {"redirect_url", redirectUrl},
                {"response_type", "code"},
                {"scope", scope},
                {"code_challenge", codeChang},
                {"code_challenge_method", "S256"}
            };

            var url = QueryHelpers.AddQueryString(OAuthEndPoint, queryParams);
            return url;
        }

        public object RefreshToken()
        {

            throw new NotImplementedException();
        }

        public async Task<OAuthResultModel> ExechangeCodeOauthT(string code, string codeVerifier, string redirectUrl)
        {
            using (var client = new HttpClient())
            {
                var tokenEndPoint = "https://oauth2.googleapis.com/token";

                var authParams = new Dictionary<string, string>
                {
                    {"client_id", ClientID},
                    {"client_secret", ClientSecret},
                    {"code", code},
                    {"code_verifier", codeVerifier},
                    {"grant_type", "authorization_code"},
                    {"redirect_uri", redirectUrl}
                };

                var content = new FormUrlEncodedContent(authParams);

                var response = await client.PostAsync(tokenEndPoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<OAuthResultModel>(responseContent);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {errorContent}");
                    return null;
                }
            }
        }
    }
}