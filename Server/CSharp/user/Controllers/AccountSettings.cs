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
using SessionService;

namespace user.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountSettings : Controller
    {
        private readonly IHASH256 _hash;
        private readonly IArgon2Hasher _hasher;
        private readonly AppDbContext _context;
        private readonly ISessionService _session;

        public AccountSettings(AppDbContext context, IArgon2Hasher hasher,  IHASH256 hash, ISessionService session)
        {
            _context = context;
            _hash = hash;
            _hasher = hasher;
            _session = session;
        }



        [HttpPut("SessionsUpdate")]
        public async Task<IActionResult> SessionsUpdate()
        {
            try
            {
                bool value = await _session.IsSessionValidAsync(Request);
                if (!value) return Unauthorized();

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
                UserModel user = await _session.GetUserDataAsync(Request);
                if (user == null) { return Unauthorized(); }

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
                string value = await _session.GetUserIdAsync(Request);
                if (value == null) return Unauthorized();

                return Ok(new { id = value });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
