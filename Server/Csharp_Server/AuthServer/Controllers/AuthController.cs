using PGAdminDAL;
using PGAdminDAL.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthServer.Models.Users;
using AuthServer.Interface.Hash;
using AuthServer.Interface.Sending;
using AuthServer.Interface.Controlles;
using AuthServer.Models.Tokens;
using System.Text.RegularExpressions;

namespace AuthServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Auth : Controller, IAuthController
    {
        private readonly IEmailSeding _emailSend;
        private readonly IJwt _jwt;
        private readonly IHASH _hash;
        private readonly IRSAHash _rsa;
        private readonly AppDbContext _context;

        public Auth(AppDbContext context, IEmailSeding emailSend, IJwt jwt, IHASH hash, IRSAHash rsa)
        {
            _context = context;
            _emailSend = emailSend;
            _jwt = jwt;
            _hash = hash;
            _rsa = rsa;
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
                        PublicKey = _rsa.GeneratePublicKeys(), 
                        PrivateKey = _rsa.GeneratePrivateKeys()

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
                        
                        await _context.SaveChangesAsync();
                        //await _emailSend.PasswordCheckEmailAsync(_user.Email, _jwt.GenerateJwtToken(userId, KeyG, 1), Request.Scheme, Request.Host.ToString());
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
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (string.IsNullOrWhiteSpace(_user.Email) || string.IsNullOrWhiteSpace(_user.Password) || !Regex.IsMatch(_user.Email, emailPattern)) { return BadRequest(new { message = "Email and Password cannot be null or empty" }); }
            if (_user.Password.Contains(" ")) { return BadRequest(new { message = "Password cannot contain spaces" }); }

            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == _user.Email);
            if (user == null) { return NotFound(new { message = "User not found." }); }

            var encryptedPassword = _hash.Encrypt(_user.Password, user.ConcurrencyStamp);
            if (user.PasswordHash != encryptedPassword) { return Unauthorized(new { message = "Invalid credentials." }); }

            var token = _jwt.GenerateJwtToken(user.Id, user.ConcurrencyStamp, 720, "User");
            return Ok(new { token });
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
                        await _context.SaveChangesAsync();
                        var accets = _jwt.GenerateJwtToken(id, user.ConcurrencyStamp, 1, userRole.RoleId);
                        return Ok(new { token = accets });
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
