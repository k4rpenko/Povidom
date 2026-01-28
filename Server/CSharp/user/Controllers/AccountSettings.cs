using PGAdminDAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using user.Models.Users;
using user.Models.Tokens;
using user.utils;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Hash.Interface;
using PGAdminDAL.Model;
using user.Models.MessageChat;

namespace user.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountSettings : Controller
    {
        private readonly IJwt _jwt;
        private readonly IHASH256 _hash;
        private readonly IArgon2Hasher _hasher;
        private readonly AppDbContext _context;

        public AccountSettings(AppDbContext context, IJwt jwt, IArgon2Hasher hasher,  IHASH256 hash)
        {
            _context = context;
            _jwt = jwt;
            _hash = hash;
            _hasher = hasher;
        }



        [HttpPut("SessionsUpdate")]
        public async Task<IActionResult> SessionsUpdate()
        {
            try
            {
                if (!Request.Cookies.TryGetValue("_ASA", out string cookieValue))
                {
                    return Unauthorized("No _ASA cookie found");
                }

                var sessions = await _context.Sessions
                    .FirstOrDefaultAsync(u => u.KeyHash == cookieValue);

                if (sessions == null || sessions.LoginTime < DateTime.UtcNow)
                {
                    _context.Sessions.Remove(sessions);
                    await _context.SaveChangesAsync();
                    return Unauthorized("Session expired or not found");
                }
                
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> CheckingPassword(AccountSettingsModel Account)
        {
            try
            {
                if (Account.Password != null  && _jwt.ValidateToken(Account.Token, _context))
                {
                    var id = _jwt.GetUserIdFromToken(Account.Token);
                    var user = await _context.Users.FindAsync(id);
                    if (user != null)
                    {
                        string HashNewPassword = _hash.Encrypt(Account.NewPassword, user.ConcurrencyStamp);
                        string HashPassword = _hash.Encrypt(Account.Password, user.ConcurrencyStamp);
                        if (HashPassword == user.PasswordHash)
                        {
                            user.PasswordHash = HashNewPassword;
                            await _context.SaveChangesAsync();
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
                if (!Request.Cookies.TryGetValue("_ASA", out string cookieValue))
                {
                    return Unauthorized("No _ASA cookie found");
                }

                var sessions = await _context.Sessions
                    .FirstOrDefaultAsync(u => u.KeyHash == cookieValue);

                var id = sessions.UserId;
                var user = await _context.Users.FindAsync(id);

                if (user == null) { return NotFound(new { message = "User not found" }); }

                if (!string.IsNullOrWhiteSpace(model.FirstName)) user.FirstName = model.FirstName;
                if (!string.IsNullOrWhiteSpace(model.Email)) user.Email = model.Email;
                if (!string.IsNullOrWhiteSpace(model.PhoneNumber)) user.PhoneNumber = model.PhoneNumber;
                if (!string.IsNullOrWhiteSpace(model.LastName)) user.LastName = model.LastName;
                if (!string.IsNullOrWhiteSpace(model.Avatar)) user.Avatar = model.Avatar;
                if (!string.IsNullOrWhiteSpace(model.NickName)) user.UserName = model.NickName.ToLower();
                if (!string.IsNullOrWhiteSpace(model.Title)) user.Title = model.Title;

                await _context.SaveChangesAsync();
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
                var mainUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.Id);

                if (mainUser == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var additionalNicknames = new UserName().GenerateAdditionalNicknames(model.NickName, _context);
                return Ok(new { modUserName = additionalNicknames });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("ID")]
        public async Task<IActionResult> GetUserID()
        {
            try
            {
                if (!Request.Cookies.TryGetValue("_ASA", out string cookieValue))
                {
                    return Unauthorized("No _ASA cookie found");
                }

                var sessions = await _context.Sessions
                    .FirstOrDefaultAsync(u => u.KeyHash == cookieValue);

                if (sessions == null || sessions.LoginTime < DateTime.UtcNow)
                {
                    return Unauthorized("Session expired or not found");
                }

                return Ok(new { id = sessions.UserId });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
