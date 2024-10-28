using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PGAdminDAL;
using Server.Models;
using System;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Fleets : Controller
    {
        private readonly AppDbContext context;
        public Fleets(AppDbContext _context) { context = _context;  }

        [HttpPost("FindPeople")]
        public async Task<IActionResult> FindPeople(string query)
        {
            try
            {

                var users = await context.User
                    .Where(u => u.UserName.ToLower().Contains(query) ||
                                u.FirstName.ToLower().Contains(query) ||
                                u.LastName.ToLower().Contains(query))
                    .Take(7)
                    .ToListAsync();

                var fleetsUsers = users.Select(u => new FleetsUserFModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Avatar = u.Avatar,
                    Title = u.Title,
                    Subscribers = u.Subscribers,
                    Followers = u.Followers,
                    PostID = u.PostID
                }).ToList();

                return Ok(fleetsUsers);
            }
            catch (Exception ex) { return StatusCode(500); }
        }
    }
}
