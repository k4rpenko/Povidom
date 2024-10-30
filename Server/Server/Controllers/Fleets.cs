using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PGAdminDAL;
using PGAdminDAL.Model;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Fleets : Controller
    {
        private readonly AppDbContext context;
        private readonly JWT _jwt = new JWT();

        public Fleets(AppDbContext _context) { context = _context; }

        [HttpGet("FindPeople")]
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
                }).ToList();

                return Ok(fleetsUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("")]
        public async Task<IActionResult> UserGet(string Nick)
        {
            try
            {
                var user = await context.User.FirstOrDefaultAsync(u => u.UserName == Nick);
                if (user != null)
                {
                    var fleetsUser = new FleetsUserFModel
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Avatar = user.Avatar,
                        Title = user.Title,
                        PostID = user.PostID
                    };

                    if (Request.Cookies.TryGetValue("Token", out string cookieValue))
                    {
                        var id = new JWT().GetUserIdFromToken(cookieValue);
                        if (id != null)
                        {
                            fleetsUser.SubscribersBool = user.Subscribers.Contains(id);
                            fleetsUser.FollowersBool = user.Followers.Contains(id);
                        }
                    }

                    return Ok(fleetsUser);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPut("Subscribers")]
        public async Task<IActionResult> Subscribers(AccountSettingsModel Account)
        {
            try
            {
                var user = await context.User.FirstOrDefaultAsync(u => u.UserName == Account.NickName);
                var id = _jwt.GetUserIdFromToken(Account.Token);
                var You = await context.User.FindAsync(id);

                if (user != null && You != null)
                {
                    user.Followers.Add(Account.Id);
                    You.Subscribers.Add(Account.NickName);

                    await context.SaveChangesAsync();
                    return Ok();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpDelete("Subscribers")]
        public async Task<IActionResult> DeleteSubscribers(AccountSettingsModel Account)
        {
            try
            {
                var user = await context.User.FirstOrDefaultAsync(u => u.UserName == Account.NickName);
                var id = _jwt.GetUserIdFromToken(Account.Token);
                var You = await context.User.FindAsync(id);

                if (user != null && You != null)
                {
                    user.Followers.Remove(Account.Id);
                    You.Subscribers.Remove(Account.NickName);

                    await context.SaveChangesAsync();
                    return Ok();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }


        [HttpPut("appeal")]
        public async Task<IActionResult> Appeal(AccountSettingsModel Account)
        {
            try
            {
                var user = context.User.FirstOrDefault(u => u.UserName == Account.NickName);

                if (user != null)
                {
                    user.Appeal.Add(Account.Appeal, Account.Id);

                    await context.SaveChangesAsync();
                    return Ok();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
