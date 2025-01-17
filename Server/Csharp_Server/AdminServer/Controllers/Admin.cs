using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PGAdminDAL;
using AdminServer.Interface.Hash;
using AdminServer.Interface.Sending;
using AdminServer.Models.Admin;


namespace AdminServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Admin : Controller
    {
        private readonly IEmailSeding _emailSend;
        private readonly IJwt _jwt;
        private readonly AppDbContext _context;

        public Admin(AppDbContext context, IJwt jwt, IEmailSeding emailSend) 
        { 
            _context = context;
            _jwt = jwt;
            _emailSend = emailSend;
        }

        [HttpGet("/")]
        public async Task<IActionResult> GetAdmin()
        {
            var jwt = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (!string.IsNullOrEmpty(jwt))
            {
                if (_jwt.ValidateToken(jwt, _context))
                {
                    var id = _jwt.GetUserIdFromToken(jwt);
                    var userRole = await _context.UserRoles.FirstOrDefaultAsync(u => u.UserId == id);
                    if (userRole != null)
                    {
                        var role = await _context.Roles.FirstOrDefaultAsync(u => u.Id == userRole.RoleId);
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
                var user = await _context.User.FirstOrDefaultAsync(u => u.Id == _admin.Id);
                if (user.LockoutEnabled)
                {
                    user.LockoutEnd = _admin.block;
                    await _context.SaveChangesAsync();
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
                var user = await _context.User.FirstOrDefaultAsync(u => u.Id == _admin.Id);
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
            var user = await _context.User.FirstOrDefaultAsync(u => u.UserName == nickname);
            if (user != null) {
                return Ok(new { User = user });
            }
            return NotFound();
        }
    }
}
