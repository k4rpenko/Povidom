using Server.Hash;
using Server.Sending;
using PGAdminDAL;
using PGAdminDAL.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedisDAL;
using Server.Models.Users;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly EmailSeding emailSend = new EmailSeding();
        private readonly AppDbContext context;
        private readonly JWT jwt = new JWT();
        HASH HASH = new HASH();
        RSAHash rsa = new RSAHash();

        public AuthController(
            AppDbContext _context
            )
        { context = _context;}


        [HttpPost("registration")]
        public async Task<IActionResult> CreateUser(UserAuth _user)
        {
            if (string.IsNullOrWhiteSpace(_user.Email) || string.IsNullOrWhiteSpace(_user.Password)) { return BadRequest(new { message = "Email and Password cannot be null or empty" }); }
            try
            {
                
                var user = context.User.FirstOrDefault(u => u.Email == _user.Email);
                if (user == null)
                {

                    var KeyG = BitConverter.ToString(HASH.GenerateKey()).Replace("-", "").ToLower();
                    int nextUserNumber = await context.User.CountAsync() + 1;
                    var newUser = new UserModel
                    {
                        Email = _user.Email,
                        ConcurrencyStamp = KeyG,
                        PasswordHash = HASH.Encrypt(_user.Password, KeyG),
                        UserName = $"User{nextUserNumber}",
                        FirstName = "User",
                        Avatar = "https://54hmmo3zqtgtsusj.public.blob.vercel-storage.com/avatar/Logo-yEeh50niFEmvdLeI2KrIUGzMc6VuWd-a48mfVnSsnjXMEaIOnYOTWIBFOJiB2.jpg",
                        PublicKey = rsa.GeneratePublicKeys(), 
                        PrivateKey = rsa.GeneratePrivateKeys()

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
                        Value = jwt.GenerateJwtToken(newUser.Id, KeyG, 720, UserRoleID.Id)
                    };

                    context.UserTokens.Add(newToken);

                    await context.SaveChangesAsync();
                   
                    var userId = newUser.Id;
                    var record = await context.User.FindAsync(userId);

                    if (record != null)
                    {
                        var RefreshToken = newToken.Value;
                        
                        await context.SaveChangesAsync();
                        await emailSend.PasswordCheckEmailAsync(_user.Email, jwt.GenerateJwtToken(userId, KeyG, 1), Request.Scheme, Request.Host.ToString());
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
                Console.WriteLine("\n\n ERROR: " + ex);
                return StatusCode(500, new { message = "An internal server error occurred." });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(UserAuth _user)
        {
            if (string.IsNullOrWhiteSpace(_user.Email) || string.IsNullOrWhiteSpace(_user.Password)) { return BadRequest(new { message = "Email or password cannot be empty." }); }
            try
            {
                var user = await context.User.FirstOrDefaultAsync(u => u.Email == _user.Email);
                if (user == null) { return NotFound(new { message = "User not found." }); }
                if (HASH.Encrypt(_user.Password, user.ConcurrencyStamp) != user.PasswordHash) { return Unauthorized(new { message = "Invalid email or password." }); }
                if (!user.EmailConfirmed) { return BadRequest(new { message = "Email not confirmed." }); }

                var roleUser = await context.UserRoles.FirstOrDefaultAsync(u => u.UserId == user.Id);
                if (roleUser == null) { return NotFound(new { message = "User role not found." }); }

                var token = jwt.GenerateJwtToken(user.Id, user.ConcurrencyStamp, 1, roleUser.RoleId);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\n ERROR: " + ex);
                return StatusCode(500, new { message = "An internal server error occurred." });
            }
        }

    }
}
