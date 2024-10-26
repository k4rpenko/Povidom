using Amazon.Runtime.Internal.Transform;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQL;
using PGAdminDAL;
using Server.Models;
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
                _data.CreatedAt = DateTime.UtcNow;
                _data.UpdatedAt = DateTime.UtcNow;
                await _customers.InsertOneAsync(_data);
                return Ok();
                
            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }

        [HttpPost("DeleytPost")]
        public async Task<IActionResult> DeleytPost(SpaceWorkModel _data)
        {
            try
            {
                var objectId = ObjectId.Parse(_data.Id);
                var deleteResult = await _customers.DeleteOneAsync(post => post.Id == objectId);
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
        public async Task<IActionResult> LikePost(SpaceWorkModel _data)
        {
            try
            {
                var user = context.User.FirstOrDefault(u => u.Id == _data.UserId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                
                var newLike = new Like()
                {
                    UserId = _data.UserId,
                    CreatedAt = DateTime.UtcNow
                };

                var objectId = ObjectId.Parse(_data.Id);

                var updateDefinition = Builders<SpacePostModel>.Update.AddToSet(post => post.Like, newLike);
                var updateResult = await _customers.UpdateOneAsync(
                    post => post.Id == objectId,
                    updateDefinition
                );

                if (updateResult.MatchedCount == 0)
                {
                    return NotFound("Post not found.");
                }

                user.LikePost.Add(_data.UserId, _data.Id);

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
