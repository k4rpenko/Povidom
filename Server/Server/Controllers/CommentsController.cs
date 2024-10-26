using Microsoft.AspNetCore.Mvc;
using PGAdminDAL;
using PGAdminDAL.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] CommentModel comment)
        {
            if (string.IsNullOrEmpty(comment.Text) || string.IsNullOrEmpty(comment.UserId) || string.IsNullOrEmpty(comment.PostId))
            {
                return BadRequest(new { message = "Comment text, user ID, and post ID are required." });
            }

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Comment added successfully", comment });
        }

        [HttpGet("GetComments/{postId}")]
        public async Task<IActionResult> GetComments(string postId)
        {
            var comments = await _context.Comments.Where(c => c.PostId == postId).ToListAsync();
            return Ok(comments);
        }
    }
}
