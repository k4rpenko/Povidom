using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using PGAdminDAL;
using PGAdminDAL.Model;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly JWT _jwt;

        public ProfileController(AppDbContext context, JWT jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        private string GetCurrentUserId()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            return _jwt.GetUserIdFromToken(token);
        }

        [HttpGet("{nickname}")]
        public async Task<IActionResult> GetUserProfile(string nickname)
        {
            var user = await _context.User
                .Include(u => u.Followers)
                .FirstOrDefaultAsync(u => u.UserName == nickname);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            bool isFollowing = user.Followers.Any(f => f.FollowerId == currentUserId);

            return Ok(new
            {
                User = new
                {
                    user.UserName,
                    user.Avatar,
                    user.FirstName,
                    user.LastName,
                    user.Title
                },
                IsFollowing = isFollowing
            });
        }

        [HttpPost("{nickname}/follow")]
        public async Task<IActionResult> FollowUser(string nickname)
        {
            var userToFollow = await _context.User.FirstOrDefaultAsync(u => u.UserName == nickname);
            var currentUserId = GetCurrentUserId();

            if (userToFollow == null || string.IsNullOrEmpty(currentUserId))
            {
                return NotFound(new { message = "User not found or invalid token." });
            }

            if (userToFollow.Followers.Any(f => f.FollowerId == currentUserId))
            {
                return BadRequest(new { message = "You are already following this user." });
            }

            var followRelation = new Follow
            {
                UserId = userToFollow.Id,
                FollowerId = currentUserId
            };

            _context.Follows.Add(followRelation);
            await _context.SaveChangesAsync();

            return Ok(new { message = "You are now following this user." });
        }

        [HttpDelete("{nickname}/unfollow")]
        public async Task<IActionResult> UnfollowUser(string nickname)
        {
            var userToUnfollow = await _context.User.FirstOrDefaultAsync(u => u.UserName == nickname);
            var currentUserId = GetCurrentUserId();

            if (userToUnfollow == null || string.IsNullOrEmpty(currentUserId))
            {
                return NotFound(new { message = "User not found or invalid token." });
            }

            var followRelation = await _context.Follows
                .FirstOrDefaultAsync(f => f.UserId == userToUnfollow.Id && f.FollowerId == currentUserId);

            if (followRelation != null)
            {
                _context.Follows.Remove(followRelation);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "You have unfollowed this user." });
        }
    }
}
