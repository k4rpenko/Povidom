using PGAdminDAL;
using PGAdminDAL.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Hash.Interface;
using authentication.Interface.Sending;
using authentication.Models.Users;
using authentication.Models.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;

namespace authentication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Auth : Controller
    {
        private readonly IEmailSeding _emailSend;
        private readonly IJwt _jwt;
        private readonly IHASH256 _hash;
        private readonly IArgon2Hasher _hasher;
        private readonly AppDbContext _context;

        public Auth(AppDbContext context, IArgon2Hasher hasher, IEmailSeding emailSend, IJwt jwt, IHASH256 hash)
        {
            _context = context;
            _emailSend = emailSend;
            _jwt = jwt;
            _hasher = hasher;
            _hash = hash;
        }


        [HttpPost("registration")]
        public async Task<IActionResult> CreateUser(UserAuth _user)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (string.IsNullOrWhiteSpace(_user.Email) || string.IsNullOrWhiteSpace(_user.Password) || !Regex.IsMatch(_user.Email, emailPattern)) { return BadRequest(new { message = "Email and Password cannot be null or empty" }); }
            try
            {
                
                var user = _context.User.FirstOrDefault(u => u.Email == _user.Email);
                if (user == null)
                {

                    var KeyG = BitConverter.ToString(_hash.GenerateKey()).Replace("-", "").ToLower();
                    int nextUserNumber = await _context.User.CountAsync() + 1;
                    var newUser = new UserModel
                    {
                        Email = _user.Email,
                        EmailConfirmed = false,
                        ConcurrencyStamp = KeyG,
                        PasswordHash = _hash.Encrypt(_user.Password, KeyG),
                        UserName = $"User{nextUserNumber}",
                        FirstName = "User",
                        LastName = "",
                        Avatar = "https://54hmmo3zqtgtsusj.public.blob.vercel-storage.com/avatar/Logo-yEeh50niFEmvdLeI2KrIUGzMc6VuWd-a48mfVnSsnjXMEaIOnYOTWIBFOJiB2.jpg",
                    };  

                    _context.User.Add(newUser);
 

                    var UserRoleID = _context.Roles.FirstOrDefault(u => u.Name == "User");

                    var UserRole = new IdentityUserRole<string>
                    {
                        UserId = newUser.Id,
                        RoleId = UserRoleID.Id
                    };


                    _context.UserRoles.Add(UserRole);

                    var newToken = new IdentityUserToken<string>
                    {
                        UserId = newUser.Id,
                        LoginProvider = "Default",
                        Name = newUser.UserName,
                        Value = _jwt.GenerateJwtToken(newUser.Id, KeyG, 720, UserRoleID.Id)
                    };

                    _context.UserTokens.Add(newToken);

                    await _context.SaveChangesAsync();
                   
                    var userId = newUser.Id;
                    var record = await _context.User.FindAsync(userId);

                    if (record != null)
                    {
                        var RefreshToken = newToken.Value;

                        //await _emailSend.PasswordCheckEmailAsync(_user.Email, _jwt.GenerateJwtToken(userId, KeyG, 1), Request.Scheme, Request.Host.ToString());

                        string token;
                        string key;
                        bool isUnique = false;
                        do
                        {
                            key = _hasher.GenerateKey();
                            token = _hasher.GenerateHash(userId, key);

                            var find = await _context.User
                                .Where(u => u.Sessions.Any(s => s.KeyHash == token))
                                .FirstOrDefaultAsync();

                            isUnique = (find == null);

                        } while (!isUnique);

                        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                        string safeIpAddress = string.IsNullOrEmpty(ipAddress) ? "Невідома IP" : ipAddress;

                        string deviceInfo = HttpContext.Request.Headers["User-Agent"].ToString() ?? "Невідомий пристрій";

                        var SessionsData = new Sessions
                        {
                            UserId = record.Id,
                            DeviceInfo = deviceInfo,
                            IPAddress = safeIpAddress,
                            KeyHash = token,
                            Salt = key,
                            LoginTime = DateTime.UtcNow
                        };

                        record.Sessions.Add(SessionsData);

                        Response.Cookies.Append("_ASA", token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTime.UtcNow.AddHours(1)
                        });

                        await _context.SaveChangesAsync();
                        return Ok(new { message = "Registration successful" });
                    }
                }
                if (user.EmailConfirmed == false)
                {
                    return BadRequest();
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\n ERROR: " + ex);
                return StatusCode(500, new { message = "An internal server error occurred." });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(UserAuth _user)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (string.IsNullOrWhiteSpace(_user.Email) || string.IsNullOrWhiteSpace(_user.Password) || !Regex.IsMatch(_user.Email, emailPattern)) { return BadRequest(new { message = "Email and Password cannot be null or empty" }); }
            if (_user.Password.Contains(" ")) { return BadRequest(new { message = "Password cannot contain spaces" }); }

            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == _user.Email);
            if (user == null) { return NotFound(new { message = "User not found." }); }

            var encryptedPassword = _hash.Encrypt(_user.Password, user.ConcurrencyStamp);
            if (user.PasswordHash != encryptedPassword) { return Unauthorized(new { message = "Invalid credentials." }); }

            string token;
            string key;
            bool isUnique = false;
            do
            {
                key = _hasher.GenerateKey();
                token = _hasher.GenerateHash(user.Id, key);

                var find = await _context.User
                    .Where(u => u.Sessions.Any(s => s.KeyHash == token))
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
                KeyHash = token,
                Salt = key,
                LoginTime = DateTime.UtcNow
            };

            user.Sessions.Add(SessionsData);

            Response.Cookies.Append("_ASA", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            await _context.SaveChangesAsync();
            return Ok(new { message = "Registration successful" });
        }


        [HttpPost("ConfirmationAccount")]
        public async Task<IActionResult> ConfirmationAccount(TokenModel Account)
        {
            try
            {
                if (Account.ConfirmationToken != null && _jwt.ValidateToken(Account.ConfirmationToken, _context))
                {
                    var id = _jwt.GetUserIdFromToken(Account.ConfirmationToken);
                    var user = _context.User.FirstOrDefault(u => u.Id == id);
                    var userRole = _context.UserRoles.FirstOrDefault(u => u.UserId == id);
                    if (user != null)
                    {
                        user.EmailConfirmed = true;

                        string token;
                        string key;
                        bool isUnique = false;
                        do
                        {
                            key = _hasher.GenerateKey();
                            token = _hasher.GenerateHash(user.Id, key);

                            var find = await _context.User
                                .Where(u => u.Sessions.Any(s => s.KeyHash == token))
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
                            KeyHash = token,
                            Salt = key,
                            LoginTime = DateTime.UtcNow
                        };

                        Response.Cookies.Append("_ASA", token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTime.UtcNow.AddHours(1)
                        });

                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }
    }
}
