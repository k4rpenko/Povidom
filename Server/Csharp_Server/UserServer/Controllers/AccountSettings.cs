using PGAdminDAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserServer.Models.Users;
using UserServer.Models.Tokens;
using UserServer.Interface.Hash;
using UserServer.utils;
using System.Text.RegularExpressions;
using KafkaLibrary.Consumers;
using Microsoft.Extensions.Configuration;
using KafkaLibrary.Producers;

namespace UserServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountSettings : Controller
    {
        private readonly IJwt _jwt;
        private readonly IHASH _hash;
        private readonly AppDbContext _context;

        public AccountSettings(AppDbContext context, IJwt jwt, IHASH hash)
        {
            _context = context;
            _jwt = jwt;
            _hash = hash;
        }



        [HttpPut("TokenUpdate")]
        public async Task<IActionResult> AccessToken(TokenModel _tokenM)
        {
            try
            {
                var id = _jwt.GetUserIdFromToken(_tokenM.AccessToken);
                var user = _context.User.FirstOrDefault(u => u.Id == id);
                var userRoles = _context.UserRoles.FirstOrDefault(u => u.UserId == id);
                var refreshToke = _context.UserTokens.FirstOrDefault(t => t.UserId == id);

                if (_jwt.ValidateToken(refreshToke.Value, _context) == false)
                {
                    refreshToke.Value = null;
                    await _context.SaveChangesAsync();
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
                if (Account.Password != null  && _jwt.ValidateToken(Account.Token, _context))
                {
                    var id = _jwt.GetUserIdFromToken(Account.Token);
                    var user = await _context.User.FindAsync(id);
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
                var id = _jwt.GetUserIdFromToken(model.Token);
                var user = await _context.User.FindAsync(id);

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
                var mainUser = await _context.User.FirstOrDefaultAsync(u => u.Id == model.Id);

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
    }
}
