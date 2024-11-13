using Server.Hash;
using Server.Sending;
using PGAdminDAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.utils;
using Server.Models.Users;
using Server.Models.Tokens;
namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountSettings : Controller
    {
        private UserName _untilUser = new UserName();
        private readonly EmailSeding _emailSend = new EmailSeding();
        private readonly AppDbContext context;
        private HASH _HASH = new HASH();
        private JWT _jwt = new JWT();
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

        [HttpPut("TokenUpdate")]
        public async Task<IActionResult> AccessToken(TokenModel _tokenM)
        {
            try
            {
                var id = _jwt.GetUserIdFromToken(_tokenM.AccessToken);
                var user = context.User.FirstOrDefault(u => u.Id == id);
                var userRoles = context.UserRoles.FirstOrDefault(u => u.UserId == id);
                var refreshToke = context.UserTokens.FirstOrDefault(t => t.UserId == id);
                if (_jwt.ValidateToken(refreshToke.Value, context) == false)
                {
                    refreshToke.Value = null;
                    await context.SaveChangesAsync();
                    return Unauthorized();
                }
                var accessToken = _jwt.GenerateJwtToken(id, user.ConcurrencyStamp, 1, userRoles.RoleId);
                return Ok(new { token = accessToken });
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> CheckingPassword(AccountSettingsModel Account)
        {
            try
            {
                if (Account.Password != null  && _jwt.ValidateToken(Account.Token, context))
                {
                    var id = _jwt.GetUserIdFromToken(Account.Token);
                    var user = await context.User.FindAsync(id);
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

        [HttpPut("UpdateData")]
        public async Task<IActionResult> UpdateUser(AccountSettingsModel model)
        {
            try
            {
                var id = _jwt.GetUserIdFromToken(model.Token);
                var user = await context.User.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                if (!string.IsNullOrWhiteSpace(model.FirstName)) user.FirstName = model.FirstName;
                if (!string.IsNullOrWhiteSpace(model.Email)) user.Email = model.Email;
                if (!string.IsNullOrWhiteSpace(model.PhoneNumber)) user.PhoneNumber = model.PhoneNumber;
                if (!string.IsNullOrWhiteSpace(model.LastName)) user.LastName = model.LastName;
                if (!string.IsNullOrWhiteSpace(model.Avatar)) user.Avatar = model.Avatar;
                if (!string.IsNullOrWhiteSpace(model.NickName)) user.UserName = model.NickName;
                if (!string.IsNullOrWhiteSpace(model.Title)) user.Title = model.Title;

                await context.SaveChangesAsync();
                return Ok(new { message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("NickName")]
        public async Task<IActionResult> GetUserProfile(AccountSettingsModel model)
        {
            try
            {
                var mainUser = await context.User.FirstOrDefaultAsync(u => u.Id == model.Id);

                if (mainUser == null)
                {
                    return Ok();
                }
                else {
                    var additionalNicknames = _untilUser.GenerateAdditionalNicknames(model.NickName, context);

                    return Ok(new { modUserName = additionalNicknames });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
