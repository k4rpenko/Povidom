using Amazon.Runtime.Internal.Transform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQL;
using PGAdminDAL;
using Server.Models.Post;
using Server.Protection;
using System.Linq;
using System.Xml.Linq;
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
        private readonly JWT _jwt = new JWT();

        [HttpPost("AddPost")]
        public async Task<IActionResult> AddPost(SpacePostModel _data)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(_data.UserId);
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                _data.CreatedAt = DateTime.UtcNow;
                _data.UpdatedAt = DateTime.UtcNow;
                await _customers.InsertOneAsync(_data);

                user.PostID.Add(_data.Id.ToString());
                await context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }

        [HttpDelete("DeleytPost")]
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

        [HttpPut("LikePost")]
        public async Task<IActionResult> LikePost(SpaceWorkModel _data)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(_data.UserId);
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);

                Console.WriteLine(id);

                if (user.LikePostID.Contains(_data.Id))
                {
                    return NotFound();
                }
                
                var newLike = new Like()
                {
                    UserId = _data.UserId,
                    CreatedAt = DateTime.UtcNow
                };
                Console.WriteLine(newLike);
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
                

                user.LikePostID.Add(_data.Id);

                await context.SaveChangesAsync();

                return Ok("Post liked successfully.");

            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }

        [HttpPut("Comment")]
        public async Task<IActionResult> AddComment(SpaceWorkModel _data)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(_data.UserId);
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
                var objectId = ObjectId.Parse(_data.Id);
                var post = await _customers.Find(post => post.Id == objectId).FirstOrDefaultAsync();

                if (post == null)
                {
                    return NotFound("Post not found");
                }


                var newComment = new Comment
                {
                    UserId = _data.UserId,
                    Content = _data.Content,
                    CreatedAt = DateTime.UtcNow
                };

                post.Comments.Add(newComment);
                var filter = Builders<SpacePostModel>.Filter.Eq(p => p.Id, post.Id);

                user.CommentPostID.Add(post.Id.ToString());


                await _customers.ReplaceOneAsync(filter, post);
                await context.SaveChangesAsync();

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        [HttpPut("Retpost")]
        public async Task<IActionResult> Retweet(SpaceWorkModel _data)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(_data.UserId);
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == id); ;

                var objectId = ObjectId.Parse(_data.Id);

                var SpacePostModel = new SpacePostModel()
                {
                    UserId = id,
                    Content = _data.Content,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    MediaUrls = _data.MediaUrls,
                    Hashtags = _data.Hashtags,
                    Mentions = _data.Mentions,
                };
                await _customers.InsertOneAsync(SpacePostModel);
                var RetweetPost = SpacePostModel.Id;

                //OriginalPost
                var updateDefinition = Builders<SpacePostModel>.Update.AddToSet(post => post.InRetpost, RetweetPost.ToString());
                var updateResult = await _customers.UpdateOneAsync(
                    post => post.Id == objectId,
                    updateDefinition
                );

                //RetweetPost
                var updateDefinitionRetweet = Builders<SpacePostModel>.Update.AddToSet(post => post.Retpost, objectId.ToString());
                var updateResultRetweet = await _customers.UpdateOneAsync(
                    post => post.Id == RetweetPost,
                    updateDefinitionRetweet
                );

                if (updateResult.MatchedCount == 0)
                {
                    return NotFound("Post not found.");
                }

                user.RetweetPostID.Add(_data.Id);
                await context.SaveChangesAsync();

                return Ok("Post liked successfully.");

            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }

        [HttpGet("Home")]
        public async Task<IActionResult> Home()
        {
            //var filter = Builders<SpacePostModel>.Filter.ElemMatch(post => post.Views, Builders<string>.Filter.Ne(view => view, _data.Id.ToString()));
            //var posts = await _customers.Find(filter).Limit(30).ToListAsync();

            List<SpacePostModel> posts = await _customers.Find(_ => true).Limit(30).ToListAsync();

            if (!Request.Cookies.TryGetValue("authToken", out string cookieValue))
            {
                return Unauthorized();
            }
            var id = new JWT().GetUserIdFromToken(cookieValue);

            foreach (var item in posts)
            {
                if (!item.Views.Contains(id))
                {
                    item.Views.Add(id);
                    await _customers.ReplaceOneAsync(
                        filter => filter.Id == item.Id,
                        item 
                    );
                }
            }

            List<PostHome> postHomeList = posts.Select(post => new PostHome
            {
                Id = post.Id.ToString().ToString(),
                UserId = post.UserId,
                UserNickname = post.UserNickname,
                UserName = post.UserName,
                UserAvatar = post.UserAvatar,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                MediaUrls = post.MediaUrls?.Count ?? 0,
                Like = post.Like?.Count ?? 0,
                Retpost = post.Retpost?.Count ?? 0,
                InRetpost = post.InRetpost?.Count ?? 0,
                Hashtags = post.Hashtags?.Count ?? 0,
                Mentions = post.Mentions?.Count ?? 0,
                Comments = post.Comments?.Count ?? 0,
                Views = post.Views?.Count ?? 0,
                SPublished = post.SPublished
            }).ToList();

            return Ok(new { Post = postHomeList });

        }

        [HttpGet("")]
        public async Task<IActionResult> Home(string UserNick)
        {
            var filter = Builders<SpacePostModel>.Filter.Eq(post => post.UserNickname, UserNick);
            List<SpacePostModel> posts = await _customers.Find(_ => true).Limit(30).ToListAsync();

            List<PostHome> postHomeList = posts.Select(post => new PostHome
            {
                Id = post.Id.ToString().ToString(),
                UserId = post.UserId,
                UserNickname = post.UserNickname,
                UserName = post.UserName,
                UserAvatar = post.UserAvatar,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                MediaUrls = post.MediaUrls?.Count ?? 0,
                Like = post.Like?.Count ?? 0,
                Retpost = post.Retpost?.Count ?? 0,
                InRetpost = post.InRetpost?.Count ?? 0,
                Hashtags = post.Hashtags?.Count ?? 0,
                Mentions = post.Mentions?.Count ?? 0,
                Comments = post.Comments?.Count ?? 0,
                Views = post.Views?.Count ?? 0,
                SPublished = post.SPublished
            }).ToList();

            return Ok(new { Post = postHomeList });
        }
    }
}
