using Server.Hash;
using Server.Models;
using Server.Sending;
using PGAdminDAL;
using PGAdminDAL.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedisDAL;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly EmailSeding _emailSend = new EmailSeding();
        private readonly AppDbContext context;
        private readonly RedisConfigure redis;
        private readonly JWT _jwt = new JWT();
        HASH _HASH = new HASH();
        public AuthController(AppDbContext _context, RedisConfigure _redis) { context = _context; redis = _redis; }



        [HttpPost("registration")]
        public async Task<IActionResult> CreateUser(UserAuth _user)
        {
            if (string.IsNullOrWhiteSpace(_user.Email) || string.IsNullOrWhiteSpace(_user.Password)) { return BadRequest(new { message = "Email and Password cannot be null or empty" }); }
            try
            {
                
                var user = context.User.FirstOrDefault(u => u.Email == _user.Email);
                if (user == null)
                {

                    var KeyG = BitConverter.ToString(_HASH.GenerateKey()).Replace("-", "").ToLower();
                    int nextUserNumber = await context.User.CountAsync() + 1;
                    var newUser = new UserModel
                    {
                        Email = _user.Email,
                        ConcurrencyStamp = KeyG,
                        PasswordHash = _HASH.Encrypt(_user.Password, KeyG),
                        UserName = $"User{nextUserNumber}",
                        FirstName = "User",
                        Avatar = "https://54hmmo3zqtgtsusj.public.blob.vercel-storage.com/avatar/Logo-yEeh50niFEmvdLeI2KrIUGzMc6VuWd-a48mfVnSsnjXMEaIOnYOTWIBFOJiB2.jpg"
                    };  

                    context.User.Add(newUser);
 

                    var UserRoleID = context.Roles.FirstOrDefault(u => u.Name == "User");

                    var UserRole = new IdentityUserRole<string>
                    {
                        UserId = newUser.Id,
                        RoleId = UserRoleID.Id
                    };


                    context.UserRoles.Add(UserRole);

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
                        await _emailSend.PasswordCheckEmailAsync(_user.Email, _jwt.GenerateJwtToken(userId, KeyG, 1), Request.Scheme, Request.Host.ToString());
                        return Ok();
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
                throw new Exception("", ex);
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(UserAuth _user)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (redis.AuthRedisUser(ipAddress))
            {
                if (string.IsNullOrWhiteSpace(_user.Email) || string.IsNullOrWhiteSpace(_user.Password)) { return BadRequest(); }
                try
                {
                    var user = context.User.FirstOrDefault(u => u.Email == _user.Email);
                    var RoleUser = context.UserRoles.FirstOrDefault(u => u.UserId == user.Id);
                    if (user == null) { return NotFound(); }
                    if (_HASH.Encrypt(_user.Password, user.ConcurrencyStamp) != user.PasswordHash) { return Unauthorized(); }
                    if (user.EmailConfirmed == false)
                    {
                        return BadRequest();
                    }
                    var accets = _jwt.GenerateJwtToken(user.Id, user.ConcurrencyStamp, 1, RoleUser.RoleId);
                    return Ok(new { token = accets });
                }
                catch (Exception ex)
                {
                    throw new Exception("", ex);
                }
            }
            return StatusCode(StatusCodes.Status429TooManyRequests, new { message = "Too many requests from this IP address" });
        }
    }
}
