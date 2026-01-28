using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Identification.Models.Tokens;

namespace Identification.Sending
{
    internal class GoogleOAuth
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

        public string GenerateOauthUrl(string scope, string redirectUrl, string codeChallenge)
        {
            var OAuthEndPoint = "https://accounts.google.com/o/oauth2/v2/auth";

            var queryParams = new Dictionary<string, string>
            {
                {"client_id", ClientID},
                {"redirect_uri", redirectUrl},
                {"response_type", "code"},
                {"scope", scope},
                {"code_challenge", codeChallenge},
                {"code_challenge_method", "S256"}
            };

            var url = QueryHelpers.AddQueryString(OAuthEndPoint, queryParams);
            return url;
        }

        public async Task<OAuthResultModel> ExchangeCodeForTokens(string code, string codeVerifier, string redirectUrl)
        {
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("code is required", nameof(code));
            if (string.IsNullOrWhiteSpace(codeVerifier)) throw new ArgumentException("codeVerifier is required", nameof(codeVerifier));
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
                if (!string.IsNullOrEmpty(ClientSecret))
                {
                    authParams["client_secret"] = ClientSecret;
                }

                var content = new FormUrlEncodedContent(authParams);
                var response = await client.PostAsync(tokenEndPoint, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var token = JsonConvert.DeserializeObject<OAuthResultModel>(responseContent);
                    return token;
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