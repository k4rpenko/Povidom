using Hash;
using Hash.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using PGAdminDAL;
using PGAdminDAL.Model;
using posts.Models.MessageChat;
using posts.Models.Post;
using System.ComponentModel.Design;
using System.Linq;

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
                    Repost = 0,
                    RepostAmount = 0,
                    YouRepost = user.Repost.Contains(post.Id.ToString()) ? true : false,
                    Hashtags = 0,
                    Mentions = 0,
                    CommentAmount = 0,
                    YouComment = user.CommentsId.Any(c => c.PostId == post.Id.ToString()) ? true : false,
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

        [HttpPut("LikeComent")]
        public async Task<IActionResult> LikeComent([FromQuery] string post_id, [FromQuery] string coment_id)
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


                var newLike = new Like()
                {
                    UserId = id,
                    CreatedAt = DateTime.UtcNow
                };

                var objectId = ObjectId.Parse(post_id);
                var ComentobjectId = ObjectId.Parse(coment_id);

                var post = await _customers.Find(p => p.Id == objectId).FirstOrDefaultAsync();
                var comment = post.Comments.FirstOrDefault(c => c.Id == ComentobjectId);

                if (comment != null)
                {
                    comment.Like.Add(newLike);
                    await _customers.ReplaceOneAsync(p => p.Id == objectId, post);
                }
                else
                {
                    return NotFound(false);
                }


                if (!user.LikeComments.Any(c => c.PostId == post_id))
                {
                    user.LikeComments.Add(new Сoments
                    {
                        PostId = post_id,
                        CommentId = new List<string> { coment_id }
                    });
                }
                else
                {
                    var commentContainer = user.LikeComments.FirstOrDefault(c => c.PostId == post_id);

                    if (commentContainer != null)
                    {
                        commentContainer.CommentId.Add(coment_id);
                    }
                }

                await context.SaveChangesAsync();

                return Ok(true);

            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }

        [HttpDelete("LikeComent")]
        public async Task<IActionResult> DeleteLikeComent([FromQuery] string post_id, [FromQuery] string coment_id)
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

                var objectId = ObjectId.Parse(post_id);
                var ComentobjectId = ObjectId.Parse(coment_id);

                var post = await _customers.Find(p => p.Id == objectId).FirstOrDefaultAsync();
                var comment = post.Comments.FirstOrDefault(c => c.Id == ComentobjectId);

                if (comment != null && post != null && user.LikeComments.Any(c => c.PostId == post_id))
                {
                    comment.Like.RemoveAll(l => l.UserId == user.Id);
                    await _customers.ReplaceOneAsync(p => p.Id == objectId, post);
                }
                else
                {
                    return NotFound(false);
                }

                var commentContainer = user.LikeComments.FirstOrDefault(c => c.PostId == post_id);

                if (commentContainer != null)
                {
                    commentContainer.CommentId.Remove(coment_id);
                }

                await context.SaveChangesAsync();

                return Ok(true);

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
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == _data.UserId);
                var objectId = ObjectId.Parse(_data.Id);
                var post = await _customers.Find(post => post.Id == objectId).FirstOrDefaultAsync();

                if (post == null)
                {
                    return NotFound("Post not found");
                }


                var newComment = new Comment
                {
                    Id = ObjectId.GenerateNewId(),
                    UserId = _data.UserId,
                    Content = _data.Content,
                    CreatedAt = DateTime.UtcNow
                };

                post.Comments.Add(newComment);

                var filter = Builders<SpacePostModel>.Filter.Eq(p => p.Id, post.Id);
                var postId = post.Id.ToString();
                var commentId = newComment.Id.ToString();


                if (!user.CommentsId.Any(c => c.PostId == post.Id.ToString()))
                {
                    user.CommentsId.Add(new Сoments
                    {
                        PostId = post.Id.ToString(),
                        CommentId = new List<string> { _data.Id }
                    });
                }
                else
                {
                    var commentContainer = user.CommentsId.FirstOrDefault(c => c.PostId == post.Id.ToString());

                    if (commentContainer != null)
                    {
                        commentContainer.CommentId.Add(_data.Id);
                    }
                }


                await _customers.ReplaceOneAsync(filter, post);
                await context.SaveChangesAsync();

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("SavedPost")]
        public async Task<IActionResult> SavedPost([FromQuery] string post_id)
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
                    NotFound(false);
                }

                user.SavedPost.Add(post_id);
                await context.SaveChangesAsync();

                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while removing the like: {ex.Message}");
            }
        }

        [HttpDelete("SavedPost")]
        public async Task<IActionResult> DeleteSavedPost([FromQuery] string post_id)
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

                user.SavedPost.Remove(post_id);
                await context.SaveChangesAsync();

                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while removing the like: {ex.Message}");
            }
        }

        [HttpPut("Retpost")]
        public async Task<IActionResult> Retpost([FromQuery] string post_id)
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
                    NotFound(false);
                }

                var newRepost = new Like()
                {
                    UserId = id,
                    CreatedAt = DateTime.UtcNow
                };

                var objectId = ObjectId.Parse(post_id);
                var post = await _customers.Find(p => p.Id == objectId).FirstOrDefaultAsync();

                if (post != null)
                {
                    post.Repost.Add(newRepost);
                    await _customers.ReplaceOneAsync(p => p.Id == objectId, post);
                }
                else
                {
                    return NotFound(false);
                }

                user.Repost.Add(post_id);
                await context.SaveChangesAsync();

                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while removing the like: {ex.Message}");
            }
        }

        [HttpDelete("Retpost")]
        public async Task<IActionResult> DeleteRetpost([FromQuery] string post_id)
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
                    NotFound(false);
                }

                var objectId = ObjectId.Parse(post_id);
                var post = await _customers.Find(p => p.Id == objectId).FirstOrDefaultAsync();

                if (post != null)
                {
                    post.Repost.RemoveAll(p => p.UserId == id);
                    await _customers.ReplaceOneAsync(p => p.Id == objectId, post);
                }
                else
                {
                    return NotFound(false);
                }

                user.Repost.Remove(post_id);
                await context.SaveChangesAsync();

                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while removing the like: {ex.Message}");
            }
        }

        [HttpGet("GetPosts")]
        public async Task<IActionResult> Home()
        { 

            List<SpacePostModel> posts = await _customers.Find(_ => true).Limit(30).ToListAsync();
            var PostList = new List<PostHome>();

            foreach (var post in posts)
            {
                var CreatorData = context.Users.FirstOrDefault(u => u.Id == post.UserId);
                var Creator = new UserFind
                {
                    Id = CreatorData.Id,
                    UserName = CreatorData.UserName,
                    FirstName = CreatorData.FirstName,
                    Avatar = CreatorData.Avatar
                };

                var postHome = new PostHome
                {
                    Id = post.Id.ToString(),
                    User = Creator,
                    Content = post.Content,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt,
                    MediaUrls = post.MediaUrls,
                    LikeAmount = post.Like?.Count ?? 0,
                    YouLike = false,
                    RepostAmount = post.Repost?.Count ?? 0,
                    YouRepost = false,
                    Hashtags = post.Hashtags?.Count ?? 0,
                    Mentions = post.Mentions?.Count ?? 0,
                    CommentAmount = post.Comments?.Count ?? 0,
                    YouComment = false,
                    ViewsAmount = post.Views.Count,
                    SPublished = post.SPublished
                };

                PostList.Add(postHome);
            }

            if (Request.Cookies.TryGetValue("_ASA", out string cookieValue))
            {
                var sessions = await context.Sessions.FirstOrDefaultAsync(u => u.KeyHash == cookieValue);

                var id = sessions.UserId;
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);



                foreach (var post in PostList)
                {
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

                    var UserConst = new UserFind
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        Avatar = user.Avatar
                    };

                    post.YouLike = user != null ? user.LikePostID.Contains(post.Id.ToString()) ? true : false : false;
                    post.YouRepost = user != null ? user.Repost.Contains(post.Id.ToString()) ? true : false : false;
                    post.YouComment = user != null ? user.CommentsId.Any(c => c.PostId == post.Id.ToString()) ? true : false : false;
                    post.YouView = true;
                    post.YouSaved = user.SavedPost.Contains(post.Id.ToString()) ? true : false;
                }
            }

            return Ok(new { Post = PostList });
        }

        [HttpGet("GetPostsById")]
        public async Task<IActionResult> PostId([FromQuery] string post_id)
        {
            if(post_id == null) return NotFound(false);

            var objectId = ObjectId.Parse(post_id);
            var post = await _customers.Find(p => p.Id == objectId).FirstOrDefaultAsync();
            var CreatorData = context.Users.FirstOrDefault(u => u.PostID.Contains(post_id));
            var Creator = new UserFind
            {
                Id = CreatorData.Id,
                UserName = CreatorData.UserName,
                FirstName = CreatorData.FirstName,
                Avatar = CreatorData.Avatar
            };

            var ComentsList = new List<PostHomeComment>();

            foreach (var item in post.Comments)
            {
                UserModel commentUserData = context.Users.FirstOrDefault(u => u.Id == item.UserId);
                UserFind commentUser = new UserFind
                {
                    Id = commentUserData.Id,
                    UserName = commentUserData.UserName,
                    FirstName = commentUserData.FirstName,
                    Avatar = commentUserData.Avatar
                };

                ComentsList.Add(new PostHomeComment
                {
                    Id = item.Id.ToString(),
                    User = commentUser,
                    Content = item.Content,
                    YouLike = false,
                    LikeAmount = item.Like?.Count ?? 0,
                });
            }

            var postHome = new PostHome
            {
                Id = post.Id.ToString(),
                User = Creator,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                MediaUrls = post.MediaUrls,
                LikeAmount = post.Like?.Count ?? 0,
                YouLike = false,
                Repost = post.Repost?.Count ?? 0,
                RepostAmount = post.Repost?.Count ?? 0,
                YouRepost = false,
                Hashtags = post.Hashtags?.Count ?? 0,
                Mentions = post.Mentions?.Count ?? 0,
                Comments = ComentsList,
                CommentAmount = post.Comments?.Count ?? 0,
                YouComment = false,
                ViewsAmount = post.Views.Count,
                SPublished = post.SPublished
            };

            if (Request.Cookies.TryGetValue("_ASA", out string cookieValue))
            {
                var sessions = await context.Sessions.FirstOrDefaultAsync(u => u.KeyHash == cookieValue);

                var id = sessions.UserId;
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);


                if (!post.Views.Contains(id))
                {
                    post.Views.Add(id);
                    await _customers.ReplaceOneAsync(
                        filter => filter.Id == post.Id,
                        post
                    );
                }

                var UserConst = new UserFind
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    Avatar = user.Avatar
                };

                postHome.YouLike = user != null ? user.LikePostID.Contains(post.Id.ToString()) ? true : false : false;
                postHome.YouRepost = user != null ? user.Repost.Contains(post.Id.ToString()) ? true : false : false;
                postHome.YouComment = user != null ? user.CommentsId.Any(c => c.PostId == post.Id.ToString()) ? true : false : false;
                postHome.YouView = true;
                postHome.YouSaved = user.SavedPost.Contains(post.Id.ToString()) ? true : false;

                foreach (var comment in postHome.Comments)
                {
                    comment.YouLike = user != null && user.LikeComments.FirstOrDefault(c => c.PostId == post.Id.ToString())?.CommentId.Contains(comment.Id.ToString()) == true;
                }
            }

            return Ok(new { Post = postHome });
        }

        [HttpGet("GetSavedPosts")]
        public async Task<IActionResult> GetSavedPosts()
        {
            if (!Request.Cookies.TryGetValue("_ASA", out string cookieValue))
            {
                return Unauthorized();
            }
            var sessions = await context.Sessions
                .FirstOrDefaultAsync(u => u.KeyHash == cookieValue);

            var id = sessions.UserId;
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

            List<PostHome> postSavedList = new List<PostHome>();

            foreach (var item in user.SavedPost)
            {
                var objectId = ObjectId.Parse(item);
                var post = await _customers.Find(p => p.Id == objectId).FirstOrDefaultAsync();

                var CreatorData = context.Users.FirstOrDefault(u => u.PostID.Contains(item));
                var Creator = new UserFind
                {
                    Id = CreatorData.Id,
                    UserName = CreatorData.UserName,
                    FirstName = CreatorData.FirstName,
                    Avatar = CreatorData.Avatar
                };

                var NewPost = new PostHome
                {
                    Id = post.Id.ToString().ToString(),
                    User = Creator,
                    Content = post.Content,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt,
                    MediaUrls = post.MediaUrls,
                    LikeAmount = post.Like?.Count ?? 0,
                    ViewsAmount = post.Views.Count,
                    YouLike = user.LikePostID.Contains(post.Id.ToString()) ? true : false,
                    Repost = post.Repost?.Count ?? 0,
                    RepostAmount = post.Repost?.Count ?? 0,
                    YouRepost = user.Repost.Contains(post.Id.ToString()) ? true : false,
                    YouSaved = user.SavedPost.Contains(post.Id.ToString()) ? true : false,
                    Hashtags = post.Hashtags?.Count ?? 0,
                    Mentions = post.Mentions?.Count ?? 0,
                    CommentAmount = post.Comments?.Count ?? 0,
                    YouComment = user.CommentsId.Any(c => c.PostId == post.Id.ToString()) ? true : false,
                    SPublished = post.SPublished
                };

                postSavedList.Add(NewPost);
            }

            return Ok(new { Post = postSavedList });
        }

        [HttpGet("GetUserPost")]
        public async Task<IActionResult> GetUserPost([FromQuery] string user_name)
        {
            if (user_name == null) return NotFound(false);

            var CreatorData = context.Users.FirstOrDefault(u => u.UserName == user_name);
            var Creator = new UserFind
            {
                Id = CreatorData.Id,
                UserName = CreatorData.UserName,
                FirstName = CreatorData.FirstName,
                Avatar = CreatorData.Avatar
            };

            List<SpacePostModel> posts = await _customers.Find(p => p.UserId == CreatorData.Id).Limit(30).ToListAsync();
            var PostList = new List<PostHome>();

            foreach (var post in posts)
            {
                var postHome = new PostHome
                {
                    Id = post.Id.ToString(),
                    User = Creator,
                    Content = post.Content,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt,
                    MediaUrls = post.MediaUrls,
                    LikeAmount = post.Like?.Count ?? 0,
                    YouLike = false,
                    Repost = post.Repost?.Count ?? 0,
                    RepostAmount = post.Repost?.Count ?? 0,
                    YouRepost = false,
                    Hashtags = post.Hashtags?.Count ?? 0,
                    Mentions = post.Mentions?.Count ?? 0,
                    CommentAmount = post.Comments?.Count ?? 0,
                    YouComment = false,
                    ViewsAmount = post.Views.Count,
                    SPublished = post.SPublished
                };

                PostList.Add(postHome);
            }

            if (Request.Cookies.TryGetValue("_ASA", out string cookieValue))
            {
                var sessions = await context.Sessions.FirstOrDefaultAsync(u => u.KeyHash == cookieValue);

                var id = sessions.UserId;
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);



                foreach (var post in PostList)
                {
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

                    var UserConst = new UserFind
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        Avatar = user.Avatar
                    };

                    post.YouLike = user != null ? user.LikePostID.Contains(post.Id.ToString()) ? true : false : false;
                    post.YouRepost = user != null ? user.Repost.Contains(post.Id.ToString()) ? true : false : false;
                    post.YouComment = user != null ? user.CommentsId.Any(c => c.PostId == post.Id.ToString()) ? true : false : false;
                    post.YouView = true;
                    post.YouSaved = user.SavedPost.Contains(post.Id.ToString()) ? true : false;
                }
            }

            return Ok(new { Post = PostList });
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
                Repost = post.Repost?.Count ?? 0,
                RepostAmount = post.Repost?.Count ?? 0,
                YouRepost = user.Repost.Contains(post.Id.ToString()) ? true : false,
                YouSaved = user.SavedPost.Contains(post.Id.ToString()) ? true : false,
                Hashtags = post.Hashtags?.Count ?? 0,
                Mentions = post.Mentions?.Count ?? 0,
                CommentAmount = post.Comments?.Count ?? 0,
                YouComment = user.CommentsId.Any(c => c.PostId == post.Id.ToString()) ? true : false,
                SPublished = post.SPublished
            }).ToList();

            return Ok(new { Post = postHomeList });
        }
    }
}
