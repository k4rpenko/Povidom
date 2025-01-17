using AuthServer.Sending;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using AuthServer.Models.Google;
using Newtonsoft.Json;
using PGAdminDAL;
using PGAdminDAL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AuthServer.Interface.Hash;

namespace AuthServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoogleAuth : Controller
    {
        GoogleOAuth GoogleOAuth = new GoogleOAuth();
        private readonly AppDbContext context;
        private readonly IJwt _jwt;
        private readonly IHASH _hash;
        private readonly IRSAHash _rsa;

        public GoogleAuth(AppDbContext _context, IJwt jwt, IHASH hash, IRSAHash rsa)
        {
            context = _context;
            _jwt = jwt;
            _hash = hash;
            _rsa = rsa;
        }

        [HttpGet("GoogleAuth")]
        public async Task<IActionResult> RedirectOauthServer()
        {
            var scope = "https://mail.google.com/ https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email";
            var redirectUrl = $"{Request.Scheme}://{Request.Host}/api/GoogleAuthGetCode";
            var codeVerifier = GenerateCodeVerifier();
            var codeChallenge = GenerateCodeChallenge(codeVerifier);
            HttpContext.Session.SetString("CodeVerifier", codeVerifier);

            var url = GoogleOAuth.GenerateOauthUrl(scope, redirectUrl, codeChallenge);
            return Redirect(url);
        }

        [HttpGet("GoogleAuthGetCode")]
        public async Task<IActionResult> CodeOauthServer(string code)
        {
            string codeVerifier = HttpContext.Session.GetString("CodeVerifier");
            var redirectUrl = $"{Request.Scheme}://{Request.Host}/api/GoogleAuthGetCode";

            var token = await GoogleOAuth.ExchangeCodeForTokens(code, codeVerifier, redirectUrl);

            var userInfo = await GetGoogleUserInfo(token.AccessToken);

            var user = context.User.FirstOrDefault(u => u.Email == userInfo.Email);
            if (user == null) 
            {
                string fullName = userInfo.Name;
                string[] nameParts = fullName.Split(' ');

                string firstName = nameParts.Length > 0 ? nameParts[0] : "";
                string lastName = nameParts.Length > 1 ? nameParts[1] : "";

                int nextUserNumber = await context.User.CountAsync() + 1;
                var KeyG = BitConverter.ToString(_hash.GenerateKey()).Replace("-", "").ToLower();
                var newUser = new UserModel
                {
                    Email = userInfo.Email,
                    EmailConfirmed = true,
                    ConcurrencyStamp = KeyG,
                    PasswordHash = "",
                    UserName = $"user{nextUserNumber}",
                    FirstName = firstName,
                    LastName = lastName,
                    Avatar = userInfo.Picture != null || userInfo.Picture != "" ? userInfo.Picture : "https://54hmmo3zqtgtsusj.public.blob.vercel-storage.com/avatar/Logo-yEeh50niFEmvdLeI2KrIUGzMc6VuWd-a48mfVnSsnjXMEaIOnYOTWIBFOJiB2.jpg",
                    PublicKey = _rsa.GeneratePublicKeys(),
                    PrivateKey = _rsa.GeneratePrivateKeys()
                };

                context.User.Add(newUser);

                var UserRoleID = context.Roles.FirstOrDefault(u => u.Name == "User");

                var UserRole = new IdentityUserRole<string>
                {
                    UserId = newUser.Id,
                    RoleId = UserRoleID.Id
                };


                context.UserRoles.Add(UserRole);

                var UserLoginService = new IdentityUserLogin<string>
                {
                    UserId = newUser.Id,
                    LoginProvider = "Google",
                    ProviderKey = userInfo.Email,
                    ProviderDisplayName = "Google"
                };

                context.UserLogins.Add(UserLoginService);

                var newToken = new IdentityUserToken<string>
                {
                    UserId = newUser.Id,
                    LoginProvider = "Default",
                    Name = newUser.UserName,
                    Value = _jwt.GenerateJwtToken(newUser.Id, KeyG, 720, UserRoleID.Id)
                };

                context.UserTokens.Add(newToken);


                await context.SaveChangesAsync();

                var userId = newUser.Id;
                var record = await context.User.FindAsync(userId);
                if (record != null)
                {
                    var RefreshToken = newToken.Value;
                    await context.SaveChangesAsync();
                }
                var accets = _jwt.GenerateJwtToken(userId, KeyG, 1, UserRole.RoleId);
                return Ok(new { token = accets });
            }
            else
            {
                var roleUser = await context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == user.Id);
                if (roleUser == null)
                {
                    return StatusCode(500, "User role not found.");
                }

                var accessToken = _jwt.GenerateJwtToken(user.Id, user.ConcurrencyStamp, 1, roleUser.RoleId);
                return Ok(new { token = accessToken });
            }
        }

        private async Task<GoogleUserInfo> GetGoogleUserInfo(string accessToken)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v3/userinfo");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var userInfo = JsonConvert.DeserializeObject<GoogleUserInfo>(jsonResponse);
                Console.WriteLine(JsonConvert.SerializeObject(userInfo, Formatting.Indented));
                return userInfo;
            }
            else
            {
                throw new Exception("Failed to retrieve user information.");
            }
        }

        private string GenerateCodeVerifier()
        {
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                var byteArray = new byte[32];
                rng.GetBytes(byteArray);
                return Convert.ToBase64String(byteArray)
                    .TrimEnd('=').Replace('+', '-').Replace('/', '_');
            }
        }

        private string GenerateCodeChallenge(string codeVerifier)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(codeVerifier));
                return Convert.ToBase64String(hash)
                    .TrimEnd('=').Replace('+', '-').Replace('/', '_');
            }
        }
    }
}