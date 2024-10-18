using Server.Hash;
using Server.Models;
using Server.Sending;
using PGAdminDAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RedisDAL;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;


namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountSettings : Controller
    {
        private readonly EmailSeding _emailSend = new EmailSeding();
        private readonly AppDbContext context;
        HASH _HASH = new HASH();
        private readonly JWT _jwt = new JWT();
        public AccountSettings(AppDbContext _context) { context = _context; }

        [HttpPost("ConfirmationAccount")]
        public async Task<IActionResult> CheckingPassword(TokenModel Account)
        {
            try
            {
                if (Account.ConfirmationToken != null && _jwt.ValidateToken(Account.ConfirmationToken, context))
                {
                    var id = _jwt.GetUserIdFromToken(Account.ConfirmationToken);
                    var user = context.User.FirstOrDefault(u => u.Id == id);
                    var userRole = context.UserRoles.FirstOrDefault(u => u.UserId == id);
                    if (user != null)
                    {
                        user.EmailConfirmed = true;
                        await context.SaveChangesAsync();
                    }
                    var accets = _jwt.GenerateJwtToken(id, user.ConcurrencyStamp, 1, userRole.RoleId);
                    return Ok(new { token = accets });
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> CheckingPassword(AccountSettingsModel Account)
        {
            try
            {
                if (Account.Password != null  && _jwt.ValidateToken(Account.Token, context))
                {
                    var id = _jwt.GetUserIdFromToken(Account.Token);
                    var user = await context.Users.FindAsync(id);
                    if (user != null)
                    {
                        string HashNewPassword = _HASH.Encrypt(Account.NewPassword, user.ConcurrencyStamp);
                        string HashPassword = _HASH.Encrypt(Account.Password, user.ConcurrencyStamp);
                        if (HashPassword == user.PasswordHash)
                        {
                            user.PasswordHash = HashNewPassword;
                            await context.SaveChangesAsync();
                            return Ok();
                        }
                        return Unauthorized("Invalid credentials");
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
