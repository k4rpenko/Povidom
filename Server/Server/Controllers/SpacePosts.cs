using Amazon.Runtime.Internal.Transform;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using NoSQL;
using PGAdminDAL;
using Server.Models;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpacePosts : Controller
    {
        private readonly IMongoCollection<SpacePostModel> _customers;
        private readonly AppDbContext context;
        public SpacePosts(AppMongoContext _Mongo, IConfiguration _configuration, AppDbContext _context) { _customers = _Mongo.Database?.GetCollection<SpacePostModel>(_configuration.GetSection("MongoDB:MongoDbDatabase").Value); context = _context; }

        [HttpPost("AddPost")]
        public async Task<IActionResult> AddPost(SpacePostModel _data)
        {
            try
            {
                await _customers.InsertOneAsync(_data);
                return Ok();
                
            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }

        [HttpPost("DeleytPost")]
        public async Task<IActionResult> DeleytPost(SpacePostModel _data)
        {
            try
            {
                var deleteResult = await _customers.DeleteOneAsync(post => post.Id == _data.Id);
                if (deleteResult.DeletedCount == 0)
                {
                    return NotFound("Post not found.");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }

        [HttpPost("LikePost")]
        public async Task<IActionResult> LikePost(SpacePostModel _data)
        {
            try
            {
                var user = context.User.FirstOrDefault(u => u.Id == _data.UserId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var newLike = new Like
                {
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow
                };


                var updateDefinition = Builders<SpacePostModel>.Update.AddToSet(post => post.Like, newLike);
                var updateResult = await _customers.UpdateOneAsync(
                    post => post.Id == _data.Id,
                    updateDefinition
                );

                if (updateResult.MatchedCount == 0)
                {
                    return NotFound("Post not found.");
                }

                user.LikePost.Add(_data.UserId, _data.Id.ToString());
                await context.SaveChangesAsync();

                return Ok("Post liked successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }
    }
}
