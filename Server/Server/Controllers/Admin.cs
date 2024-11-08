using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PGAdminDAL;
using Server.Hash;
using Server.Models.Admin;
using Server.Sending;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Admin : Controller
    {
        private readonly EmailSeding _emailSend = new EmailSeding();
        private readonly AppDbContext context;
        HASH _HASH = new HASH();
        private readonly JWT _jwt = new JWT();
        public Admin(AppDbContext _context) { context = _context; }

        [HttpGet("/")]
        public async Task<IActionResult> GetAdmin()
        {
            var jwt = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (!string.IsNullOrEmpty(jwt))
            {
                if (_jwt.ValidateToken(jwt, context))
                {
                    var id = _jwt.GetUserIdFromToken(jwt);
                    var userRole = await context.UserRoles.FirstOrDefaultAsync(u => u.UserId == id);
                    if (userRole != null)
                    {
                        var role = await context.Roles.FirstOrDefaultAsync(u => u.Id == userRole.RoleId);
                        if (role != null && (role.Name == "Admin" || role.Name == "Moderator"))
                        {
                            return Ok();
                        }
                    }
                }
            }
            return Redirect("https://localhost:8081/swagger/index.html");
        }

        [HttpPost("BlockUser")]
        public async Task<IActionResult> BlockUser(AdminModel _admin)
        {
            if(_admin.Id != null)
            {
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == _admin.Id);
                if (user.LockoutEnabled)
                {
                    user.LockoutEnd = _admin.block;
                    await context.SaveChangesAsync();
                    return Ok();
                }
            }
            return NotFound();
        }

        [HttpPut("ChangUser")]
        public async Task<IActionResult> ChangUser(AdminModel _admin)
        {
            return NotFound();
        }

        [HttpPost("SendMail")]
        public async Task<IActionResult> SendMail(AdminModel _admin)
        {
            if (_admin.Id != null)
            {
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == _admin.Id);
                if(user != null)
                {
                    await _emailSend.Writing(user.Email, _admin.SendMail);
                    return Ok();
                }
            }
            return NotFound();
        }

        [HttpGet("{nickname}")]
        public async Task<IActionResult> GetUser([FromRoute] string nickname)
        {
            var user = await context.User.FirstOrDefaultAsync(u => u.UserName == nickname);
            if (user != null) {
                return Ok(new { User = user });
            }
            return NotFound();
        }
    }
}
