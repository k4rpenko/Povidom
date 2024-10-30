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

        public Fleets(AppDbContext _context)
        {
            context = _context;
        }

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
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> UserGet(string Nick)
        {
            try
            {
                var user = await context.User
                    .FirstOrDefaultAsync(u => u.UserName == Nick);

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
                        Subscribers = (List<string>)user.Subscribers,
                        Followers = user.Followers.Select(f => f.FollowerId).ToList(), 
                        PostID = user.PostID
                    };
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
                var You = await context.User.FirstOrDefaultAsync(u => u.Id == Account.Id);

                if (user != null && You != null)
                {
                    user.Followers.Add(new Follow { FollowerId = Account.Id, UserId = user.Id });
                    You.Subscribers.Add(new Follow { FollowerId = You.Id, UserId = user.Id });

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
                var You = await context.User.FirstOrDefaultAsync(u => u.Id == Account.Id);

                if (user != null && You != null)
                {
                    var followerToRemove = user.Followers.FirstOrDefault(f => f.FollowerId == Account.Id);
                    if (followerToRemove != null)
                    {
                        user.Followers.Remove(followerToRemove);
                    }

                    var subscriberToRemove = You.Subscribers.FirstOrDefault(f => f.UserId == user.Id);
                    if (subscriberToRemove != null)
                    {
                        You.Subscribers.Remove(subscriberToRemove);
                    }

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
                var user = await context.User.FirstOrDefaultAsync(u => u.UserName == Account.NickName);

                if (user != null)
                {
                    user.Appeal[Account.Appeal] = Account.Id; 
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
