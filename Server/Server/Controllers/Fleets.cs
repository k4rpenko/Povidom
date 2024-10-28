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
                }).ToList();

                return Ok(fleetsUsers);
            }
            catch (Exception ex) { return StatusCode(500); }
        }

        [HttpPost("")]
        public async Task<IActionResult> UserGet(string Nick)
        {
            try
            {

                var user = context.User.Where(u => u.UserName == Nick).ToList();

                if (user == null)
                {
                    var fleetsUsers = user.Select(u => new FleetsUserFModel
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
                return NotFound();
            }
            catch (Exception ex) { return StatusCode(500); }
        }

        [HttpPut("Subscribers")]
        public async Task<IActionResult> Subscribers(AccountSettingsModel Account)
        {
            try
            {
                var user = context.User.FirstOrDefault(u => u.UserName == Account.NickName);
                var You = context.User.FirstOrDefault(u => u.Id == Account.Id);

                if (user == null && You == null)
                {
                    user.Followers.Add(Account.Id);
                    You.Subscribers.Add(Account.NickName);

                    await context.SaveChangesAsync();
                    return Ok();
                }
                return NotFound();
            }
            catch (Exception ex) { return StatusCode(500); }
        }

        [HttpDelete("Subscribers")]
        public async Task<IActionResult> DeleteSubscribers(AccountSettingsModel Account)
        {
            try
            {
                var user = context.User.FirstOrDefault(u => u.UserName == Account.NickName);
                var You = context.User.FirstOrDefault(u => u.Id == Account.Id);

                if (user == null && You == null)
                {
                    user.Followers.Remove(Account.Id);
                    You.Subscribers.Remove(Account.NickName);

                    await context.SaveChangesAsync();
                    return Ok();
                }
                return NotFound();
            }
            catch (Exception ex) { return StatusCode(500); }
        }

        [HttpPut("appeal")]
        public async Task<IActionResult> appeal(AccountSettingsModel Account)
        {
            try
            {
                var user = context.User.FirstOrDefault(u => u.UserName == Account.NickName);

                if (user == null)
                {
                    user.Appeal.Add(Account.Appeal, Account.Id);

                    await context.SaveChangesAsync();
                    return Ok();
                }
                return NotFound();
            }
            catch (Exception ex) { return StatusCode(500); }
        }
    }
}
