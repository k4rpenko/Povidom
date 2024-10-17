using Server.Hash;
using Server.Models;
using Server.Sending;
using PGAdminDAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RedisDAL;
using StackExchange.Redis;
using PGAdminDAL.Model;
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
                if (Account.ConfirmationToken != null && _jwt.ValidateToken(Account.ConfirmationToken))
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
                return NotFound(new { message = "Invalid Token" });
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
                if (Account.Password != null && _jwt.ValidateToken(Account.Token))
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
                return NotFound(new { message = "Invalid Token" });
            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserModel model)
        {
            try
            {
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Оновлення полів, якщо вони присутні в моделі
                if (!string.IsNullOrWhiteSpace(model.FirstName)) user.FirstName = model.FirstName;
                if (!string.IsNullOrWhiteSpace(model.LastName)) user.LastName = model.LastName;
                if (!string.IsNullOrWhiteSpace(model.Avatar)) user.Avatar = model.Avatar;
                if (!string.IsNullOrWhiteSpace(model.Nickname)) user.UserName = model.Nickname;
                if (!string.IsNullOrWhiteSpace(model.Title)) user.Title = model.Title;

                await context.SaveChangesAsync();
                return Ok(new { message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Доданий метод для отримання профілю користувача і подібних нікнеймів
        [HttpGet("profile/{nickname}")]
        public async Task<IActionResult> GetUserProfile(string nickname)
        {
            try
            {
                // Знаходимо основного користувача за нікнеймом
                var mainUser = await context.Users
                    .OfType<UserModel>() // Використовуємо UserModel
                    .FirstOrDefaultAsync(u => u.UserName == nickname);
                if (mainUser == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Генеруємо додаткові нікнейми
                var additionalNicknames = GenerateAdditionalNicknames(nickname);

                // Знаходимо користувачів за додатковими нікнеймами
                var additionalUsers = await context.Users
                    .OfType<UserModel>() // Використовуємо UserModel
                    .Where(u => additionalNicknames.Contains(u.UserName))
                    .ToListAsync();

                // Повертаємо основного користувача і список користувачів з додатковими нікнеймами
                return Ok(new
                {
                    mainUser = new
                    {
                        mainUser.UserName,
                        mainUser.FirstName,
                        mainUser.LastName,
                        mainUser.Avatar,
                        mainUser.Title
                    },
                    additionalUsers = additionalUsers.Select(user => new
                    {
                        user.UserName,
                        user.FirstName,
                        user.LastName,
                        user.Avatar
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



        // Метод для генерації додаткових нікнеймів
        private List<string> GenerateAdditionalNicknames(string nickname)
        {
            List<string> additionalNicknames = new List<string>();

            // Додаємо варіанти з додатковими символами
            additionalNicknames.Add(nickname + "1");
            additionalNicknames.Add(nickname + "8");
            additionalNicknames.Add(nickname + "_s");
            additionalNicknames.Add(nickname + "qo7");
            additionalNicknames.Add(nickname + "x");
            additionalNicknames.Add(nickname + "_z");
            additionalNicknames.Add(nickname + "_99");

            return additionalNicknames;
        }

    }
}
