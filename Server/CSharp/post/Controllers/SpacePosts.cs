    using Hash;
using Hash.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using PGAdminDAL;
using posts.Models.MessageChat;
using posts.Models.Post;

namespace posts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpacePosts : Controller
    {
        private readonly IMongoCollection<SpacePostModel> _customers;
        private readonly AppDbContext context;
        private readonly IJwt _jwt;

        public SpacePosts(AppMongoContext _Mongo, IJwt jwt,  IConfiguration _configuration, AppDbContext _context) 
        {
            var postCollectionName = _configuration["MongoDB:MongoDbDatabasePost"]; 
            _customers = _Mongo.Database.GetCollection<SpacePostModel>(postCollectionName);
            context = _context; 
        }


        [HttpPost("AddPost")]
        public async Task<IActionResult> AddPost(SpaceWorkModel _data)
        {
            try { 
                if (!Request.Cookies.TryGetValue("_ASA", out string cookieValue))
                {
                    return Unauthorized("No _ASA cookie found");
                }

                var sessions = await context.Sessions
                    .FirstOrDefaultAsync(u => u.KeyHash == cookieValue);

                var id = sessions.UserId;
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var post = new SpacePostModel
                {
                    Content = _data.Content,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SPublished = true,
                    ShaveAnswer = false
                };

                await _customers.InsertOneAsync(post);

                user.PostID.Add(post.Id.ToString());

                await context.SaveChangesAsync();

                var postHome = new PostHome
                {
                    Id = post.Id.ToString(),
                    User = new UserFind
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        Avatar = user.Avatar
                    },
                    Content = post.Content,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt,
                    MediaUrls = post.MediaUrls,
                    LikeAmount = 0,
                    YouLike = user.LikePostID.Contains(post.Id.ToString()) ? true : false,
                    Retpost = 0,
                    RetpostAmount = 0,
                    YouRetpost = user.RetweetPostID.Contains(post.Id.ToString()) ? true : false,
                    Hashtags = 0,
                    Mentions = 0,
                    CommentAmount = 0,
                    YouComment = user.CommentPostID.Contains(post.Id.ToString()) ? true : false,
                    Views = 0,
                    ViewsAmount = 0,
                    SPublished = post.SPublished,
                    ShaveAnswer = post.ShaveAnswer,
                    Ansver = post.Ansver
                };

                return Ok(new { Post = postHome });
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
        public async Task<IActionResult> LikePost([FromQuery] string post_id)
        {
            try
            {
                if (!Request.Cookies.TryGetValue("_ASA", out string cookieValue))
                {
                    return Unauthorized("No _ASA cookie found");
                }

                var sessions = await context.Sessions
                    .FirstOrDefaultAsync(u => u.KeyHash == cookieValue);

                var id = sessions.UserId;
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound(false);
                }

                if (user.LikePostID.FindIndex(id => id == post_id) >= 0)
                {
                    return Conflict(false);
                }

                var newLike = new Like()
                {
                    UserId = id,
                    CreatedAt = DateTime.UtcNow
                };

                var objectId = ObjectId.Parse(post_id);

                var updateDefinition = Builders<SpacePostModel>.Update.AddToSet(post => post.Like, newLike);
                var updateResult = await _customers.UpdateOneAsync(
                    post => post.Id == objectId,
                    updateDefinition
                );

                if (updateResult.MatchedCount == 0)
                {
                    return NotFound(false);
                }
                

                user.LikePostID.Add(post_id);

                await context.SaveChangesAsync();

                return Ok(true);

            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }

        [HttpDelete("LikePost")]
        public async Task<IActionResult> DeleteLikePost([FromQuery] string post_id)
        {
            try
            {
                if (!Request.Cookies.TryGetValue("_ASA", out string cookieValue))
                {
                    return Unauthorized(false);
                }

                var sessions = await context.Sessions
                    .FirstOrDefaultAsync(u => u.KeyHash == cookieValue);

                var id = sessions.UserId;
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound(false);
                }

                if (!user.LikePostID.Contains(post_id))
                {
                    return NotFound(false);
                }

                var objectId = ObjectId.Parse(post_id);

                var updateDefinition = Builders<SpacePostModel>.Update.PullFilter(
                    post => post.Like,
                    like => like.UserId == user.Id
                );

                var updateResult = await _customers.UpdateOneAsync(
                    post => post.Id == objectId,
                    updateDefinition
                );

                if (updateResult.MatchedCount == 0)
                {
                    return NotFound(false);
                }

                user.LikePostID.Remove(post_id);
                await context.SaveChangesAsync();

                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while removing the like: {ex.Message}");
            }
        }


        [HttpPut("Comment")]
        public async Task<IActionResult> AddComment(SpaceWorkModel _data)
        {
            try
            {
                var id = _jwt.GetUserIdFromToken(_data.UserId);
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
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
                var id = _jwt.GetUserIdFromToken(_data.UserId);
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

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

        [HttpGet("GetPosts")]
        public async Task<IActionResult> Home()
        {

            List<SpacePostModel> posts = await _customers.Find(_ => true).Limit(30).ToListAsync();
            List<PostHome> postHomeList = new List<PostHome>();

            if (Request.Cookies.TryGetValue("_ASA", out string cookieValue))
            {
                var sessions = await context.Sessions
                    .FirstOrDefaultAsync(u => u.KeyHash == cookieValue);

                var id = sessions.UserId;
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);


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
                    //var filter = Builders<SpacePostModel>.Filter.ElemMatch(post => post.Views, Builders<string>.Filter.Ne(view => view, _data.Id.ToString()));
                    //var posts = await _customers.Find(filter).Limit(30).ToListAsync();
                }

                foreach (var post in posts)
                {
                    var users = context.Users.FirstOrDefault(u => u.Id == post.UserId);

                    var postHome = new PostHome
                    {
                        Id = post.Id.ToString(),
                        User = new UserFind
                        {
                            Id = post.UserId,
                            UserName = users?.UserName,
                            FirstName = users?.FirstName,
                            Avatar = users?.Avatar
                        },
                        Content = post.Content,
                        CreatedAt = post.CreatedAt,
                        UpdatedAt = post.UpdatedAt,
                        MediaUrls = post.MediaUrls,
                        LikeAmount = post.Like?.Count ?? 0,
                        YouLike = user != null ? user.LikePostID.Contains(post.Id.ToString()) ? true : false : false,
                        Retpost = post.Retpost?.Count ?? 0,
                        RetpostAmount = post.InRetpost?.Count ?? 0,
                        YouRetpost = user != null ?  user.RetweetPostID.Contains(post.Id.ToString()) ? true : false : false,
                        Hashtags = post.Hashtags?.Count ?? 0,
                        Mentions = post.Mentions?.Count ?? 0,
                        CommentAmount = post.Comments?.Count ?? 0,
                        YouComment = user != null ? user.CommentPostID.Contains(post.Id.ToString()) ? true : false : false,
                        Views = post.Views?.Count ?? 0,
                        SPublished = post.SPublished
                    };

                    postHomeList.Add(postHome);
                }
            }

            return Ok(new { Post = postHomeList });
        }

        [HttpGet("")]
        public async Task<IActionResult> UserAccounts(string UserID)
        {
            if (!Request.Cookies.TryGetValue("_ASA", out string cookieValue))
            {
                return Unauthorized();
            }
            var id = new JWT().GetUserIdFromToken(cookieValue);
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);


            var users = await context.Users.FirstOrDefaultAsync(u => u.Id == UserID);
            var filter = Builders<SpacePostModel>.Filter.Eq(post => post.UserId, UserID);
            List<SpacePostModel> posts = await _customers.Find(_ => true).Limit(30).ToListAsync();
            var userTask = await context.Users.FirstOrDefaultAsync(u => u.Id == UserID);


            List<PostHome> postHomeList = posts.Select(post => new PostHome
            {
                Id = post.Id.ToString().ToString(),
                User = new UserFind
                {
                        Id = post.UserId,
                        UserName = users.UserName,
                        FirstName = users.FirstName,
                        Avatar = users.Avatar
                },
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                MediaUrls = post.MediaUrls,
                LikeAmount = post.Like?.Count ?? 0,
                YouLike = user.LikePostID.Contains(post.Id.ToString()) ? true : false,
                Retpost = post.Retpost?.Count ?? 0,
                RetpostAmount = post.InRetpost?.Count ?? 0,
                YouRetpost = user.RetweetPostID.Contains(post.Id.ToString()) ? true : false,
                Hashtags = post.Hashtags?.Count ?? 0,
                Mentions = post.Mentions?.Count ?? 0,
                CommentAmount = post.Comments?.Count ?? 0,
                YouComment = user.CommentPostID.Contains(post.Id.ToString()) ? true : false,
                Views = post.Views?.Count ?? 0,
                SPublished = post.SPublished
            }).ToList();

            return Ok(new { Post = postHomeList });
        }
    }
}
