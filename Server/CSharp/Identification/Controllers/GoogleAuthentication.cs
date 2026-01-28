using Hash.Interface;
using Identification.Models.Google;
using Identification.Sending;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PGAdminDAL;
using PGAdminDAL.Model;
using System.Net.Http.Headers;

namespace Identification.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoogleAuthentication : Controller
    {
        GoogleOAuth GoogleOAuth = new GoogleOAuth();
        private readonly AppDbContext context;
        private readonly IArgon2Hasher _hasher;
        private readonly IJwt _jwt;
        private readonly IHASH256 _hash;
        private readonly IRSAHash _rsa;

        public GoogleAuthentication(AppDbContext _context, IArgon2Hasher hasher, IJwt jwt, IHASH256 hash, IRSAHash rsa)
        {
            context = _context;
            _hasher = hasher;
            _jwt = jwt;
            _hash = hash;
            _rsa = rsa;
        }

        [HttpGet("GoogleAuth")]
        public async Task<IActionResult> RedirectOauthServer()
        {
            var scope = "https://mail.google.com/ https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email";
            var redirectUrl = $"{Request.Scheme}://{Request.Host}/api/GoogleAuthentication/GoogleAuthGetCode";
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
            var redirectUrl = $"{Request.Scheme}://{Request.Host}/api/GoogleAuthentication/GoogleAuthGetCode";

            var token = await GoogleOAuth.ExchangeCodeForTokens(code, codeVerifier, redirectUrl);

            var userInfo = await GetGoogleUserInfo(token.AccessToken);

            var user = context.Users.FirstOrDefault(u => u.Email == userInfo.Email);
            if (user == null) 
            {
                string fullName = userInfo.Name;
                string[] nameParts = fullName.Split(' ');

                string firstName = nameParts.Length > 0 ? nameParts[0] : "";
                string lastName = nameParts.Length > 1 ? nameParts[1] : "";

                int nextUserNumber = await context.Users.CountAsync() + 1;
                var KeyG = BitConverter.ToString(_hash.GenerateKey()).Replace("-", "").ToLower();
                var newUser = new UserModel
                {
                    Email = userInfo.Email,
                    EmailConfirmed = true,
                    ConcurrencyStamp = KeyG,
                    PasswordHash = _hasher.GenerateKey(),
                    UserName = $"user{nextUserNumber}",
                    FirstName = firstName,
                    LastName = lastName,
                    Avatar = userInfo.Picture != null || userInfo.Picture != "" ? userInfo.Picture : "https://54hmmo3zqtgtsusj.public.blob.vercel-storage.com/avatar/Logo-yEeh50niFEmvdLeI2KrIUGzMc6VuWd-a48mfVnSsnjXMEaIOnYOTWIBFOJiB2.jpg",
                };

                context.Users.Add(newUser);

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

                string _ASAToken;
                string key;
                bool isUnique = false;
                do
                {
                    key = _hasher.GenerateKey();
                    _ASAToken = _hasher.GenerateHash(newUser.Id, key);

                    var find = await context.Users
                        .Where(u => u.Sessions.Any(s => s.KeyHash == _ASAToken))
                        .FirstOrDefaultAsync();

                    isUnique = (find == null);

                } while (!isUnique);

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                string safeIpAddress = string.IsNullOrEmpty(ipAddress) ? "Невідома IP" : ipAddress;

                string deviceInfo = HttpContext.Request.Headers["User-Agent"].ToString() ?? "Невідомий пристрій";

                var SessionsData = new Sessions
                {
                    UserId = newUser.Id,
                    DeviceInfo = deviceInfo,
                    IPAddress = safeIpAddress,
                    KeyHash = _ASAToken,
                    Salt = key,
                    LoginTime = DateTime.UtcNow.AddDays(14)
                };

                context.Sessions.Add(SessionsData);

                await context.SaveChangesAsync();

                HttpContext.Response.Cookies.Append("_ASA", _ASAToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Path = "/"
                });

                return Redirect("https://localhost/home");
            }
            else
            {
                var roleUser = await context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == user.Id);
                if (roleUser == null)
                {
                    return StatusCode(500, "User role not found.");
                }

                string _ASAToken;
                string key;
                bool isUnique = false;
                do
                {
                    key = _hasher.GenerateKey();
                    _ASAToken = _hasher.GenerateHash(user.Id, key);

                    var find = await context.Users
                        .Where(u => u.Sessions.Any(s => s.KeyHash == _ASAToken))
                        .FirstOrDefaultAsync();

                    isUnique = (find == null);

                } while (!isUnique);

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                string safeIpAddress = string.IsNullOrEmpty(ipAddress) ? "Невідома IP" : ipAddress;

                string deviceInfo = HttpContext.Request.Headers["User-Agent"].ToString() ?? "Невідомий пристрій";

                var SessionsData = new Sessions
                {
                    UserId = user.Id,
                    DeviceInfo = deviceInfo,
                    IPAddress = safeIpAddress,
                    KeyHash = _ASAToken,
                    Salt = key,
                    LoginTime = DateTime.UtcNow.AddDays(14)
                };

                context.Sessions.Add(SessionsData);

                await context.SaveChangesAsync();

                HttpContext.Response.Cookies.Append("_ASA", _ASAToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Path = "/"
                });

                return Redirect("https://localhost/home");
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