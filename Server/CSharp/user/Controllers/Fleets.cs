using Hash.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using PGAdminDAL;
using PGAdminDAL.Model;
using user.Models.MessageChat;
using user.Models.Post;
using user.Models.Users;

namespace user.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Fleets : Controller
    {
        private readonly IMongoCollection<SpacePostModel> _customers;
        private readonly AppDbContext context;
        /*private readonly KafkaProducer _kafkaProducer;
        private readonly string bootstrapServers; */
        private readonly IJwt _jwt;

        public Fleets(AppDbContext _context, AppMongoContext _Mongo,  IConfiguration _configuration, IJwt jwt) 
        {
            //bootstrapServers = _configuration.GetSection("Kafka:bootstrapServers").Value;
            context = _context; 
            _customers = _Mongo.Database?.GetCollection<SpacePostModel>(_configuration.GetSection("MongoDB:MongoDbDatabase").Value);
            _jwt = jwt;
        }

        [HttpGet("FindPeople")]
        public async Task<IActionResult> FindPeople([FromQuery]  string query)
        {
            try
            {
                var users = await context.Users
                    .Where(u => u.UserName.ToLower().Contains(query) ||
                                u.FirstName.ToLower().Contains(query) ||
                                u.LastName.ToLower().Contains(query))
                    .Take(7)
                    .ToListAsync();

                if(users == null)
                {
                    return NotFound();
                }

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

        [HttpGet("chat")]
        public async Task<IActionResult> GetUsers([FromQuery]  string user_name)
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
                if (id == null) { return Unauthorized(); }


                var users = await context.Users
                    .Where(u => (u.UserName.ToLower().Contains(user_name) && u.Followers.Contains(id)  
                    || u.FirstName.ToLower().Contains(user_name) && u.Followers.Contains(id) 
                    || u.LastName.ToLower().Contains(user_name) && u.Followers.Contains(id) 
                    || u.UserName.ToLower().Contains(user_name) 
                    || u.FirstName.ToLower().Contains(user_name) 
                    || u.LastName.ToLower().Contains(user_name)) && u.Id != id)
                    .Take(7)
                    .ToListAsync();

                if (users == null)
                {
                    return NotFound();
                } 

                var fleetsUsers = users.Select(u => new UserFind
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Avatar = u.Avatar,
                    Title = u.Title,
                }).ToList();

                return Ok(new { user = fleetsUsers});
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }


        [HttpGet("Profile")]
        public async Task<IActionResult> UserGet([FromQuery] string user_name)
        {
            try
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == user_name);
                if (user != null)
                {
                    var userAccount = new UserAccount
                    {
                        Id = user.Id.ToString(),
                        Avatar = user.Avatar,
                        UserName = user.UserName,
                        Title = user.Title,
                        PhoneNumber = user.PhoneNumber,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        FollowersAmount = user.Followers.Count,
                        SubscribersAmount = user.Subscribers.Count,
                    };

                    if (!Request.Cookies.TryGetValue("_ASA", out string cookieValue))
                    {
                        return Unauthorized("No _ASA cookie found");
                    }

                    var sessions = await context.Sessions
                        .FirstOrDefaultAsync(u => u.KeyHash == cookieValue);

                    var id = sessions.UserId;
                    var You = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

                    if (You != null)
                    {
                        userAccount.YouSubscriber = user.Subscribers.Contains(id);
                        userAccount.YouFollower = user.Followers.Contains(id);
                    }

                    async Task AddPosts(IEnumerable<string> postIds, Action<PostHome> addPost)
                    {
                        foreach (var item in postIds)
                        {
                            var objectId = ObjectId.Parse(item);
                            var post = await _customers.Find(p => p.Id == objectId).FirstOrDefaultAsync();
                            if (post != null)
                            {
                                addPost(new PostHome
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
                                    LikeAmount = post.Like?.Count ?? 0,
                                    YouLike = You != null ? You.LikePostID.Contains(post.Id.ToString()) ? true : false : false,
                                    Retpost = post.Retpost?.Count ?? 0,
                                    RetpostAmount = post.InRetpost?.Count ?? 0,
                                    YouRetpost = You != null ? You.Repost.Contains(post.Id.ToString()) ? true : false : false,
                                    Hashtags = post.Hashtags?.Count ?? 0,
                                    Mentions = post.Mentions?.Count ?? 0,
                                    CommentAmount = post.Comments?.Count ?? 0,
                                    YouComment = You != null ? You.CommentsId.Any(c => c.PostId == post.Id.ToString()) ? true : false : false,
                                    Views = post.Views?.Count ?? 0,
                                    SPublished = post.SPublished
                                });
                            }
                        }
                    }


                    await AddPosts(user.PostID, post => userAccount.Post.Add(post));
                    await AddPosts(user.Repost, post => userAccount.Post.Add(post));
                    await AddPosts(user.Repost, post => userAccount.RecallPost.Add(post));

                    return Ok(new { User = userAccount });
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }

        }

        [HttpGet("")]
        public async Task<IActionResult> User()
        {
            try
            {
                    /*
                    var producer = new KafkaProducer(bootstrapServers);
                    var postKafka = await producer.SendMessage("post_topic", "key1", id);
                    SpacePostModel userPost;
                    if (postKafka != null)
                    {
                        userPost = postKafka.ToObject<SpacePostModel>();
                    }
                    else
                    {
                        userPost = null;
                    }*/
                    if (!Request.Cookies.TryGetValue("_ASA", out string cookieValue))
                    {
                        return Unauthorized("No _ASA cookie found");
                    }

                    var sessions = await context.Sessions
                        .FirstOrDefaultAsync(u => u.KeyHash == cookieValue);

                    var id = sessions.UserId;
                    var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
                    if (user != null)
                    {
                        var fleetsUser = new FleetsUserFModel
                        {
                            Id = user.Id,
                            UserName = user.UserName.ToLower(),
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Avatar = user.Avatar,
                            Title = user.Title,
                            PostID = user.PostID,
                            SubscribersAmount = user.Subscribers.Count,
                            FollowersAmount = user.Followers.Count,
                        };
                        return Ok(new { User = fleetsUser });
                    }                    
                
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Subscribers")]
        public async Task<IActionResult> Subscribers([FromQuery] string user_name, [FromQuery] int size)
        {
            try
            {
                var profile = await context.Users.FirstOrDefaultAsync(u => u.UserName == user_name);
                if (profile != null)
                {
                    List<UserAccount> UsersSubscribers = new List<UserAccount>();

                    var subscribers = profile.Subscribers
                        .Skip(size)
                        .Take(10)
                        .ToList();

                    foreach (var followerId in subscribers)
                    {
                        var userF = await context.Users.FirstOrDefaultAsync(u => u.Id == followerId);

                        if (userF != null)
                        {
                            UsersSubscribers.Add(new UserAccount
                            {
                                Id = userF.Id,
                                UserName = userF.UserName,
                                FirstName = userF.FirstName,
                                LastName = userF.LastName,
                                Avatar = userF.Avatar,
                                Title = userF.Title,
                            });
                        }
                    }

                    if (Request.Cookies.TryGetValue("_ASA", out string cookieValue))
                    {
                        var sessions = await context.Sessions.FirstOrDefaultAsync(u => u.KeyHash == cookieValue);

                        var id = sessions.UserId;
                        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
                        if (user != null)
                        {
                            foreach (var post in UsersSubscribers)
                            {
                                post.YouFollower = user.Subscribers.Contains(post.Id);
                            }
                        }
                    }

                    return Ok(new { Users = UsersSubscribers });
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Followers")]
        public async Task<IActionResult> Followers([FromQuery] string user_name, [FromQuery] int size)
        {
            try
            {
                var porofile = await context.Users.FirstOrDefaultAsync(u => u.UserName == user_name);
                if (porofile != null)
                {
                    List<UserAccount> UsersFollowers = new List<UserAccount>();

                    var Followers = porofile.Followers
                        .Skip(size)
                        .Take(10)
                        .ToList();

                    foreach (var followerId in Followers)
                    {
                        var userF = await context.Users.FirstOrDefaultAsync(u => u.Id == followerId);

                        if (userF != null) 
                        {
                            UsersFollowers.Add(new UserAccount
                            {
                                Id = userF.Id,
                                UserName = userF.UserName,
                                FirstName = userF.FirstName,
                                LastName = userF.LastName,
                                Avatar = userF.Avatar,
                                Title = userF.Title,
                            });
                        }
                    }

                    if (Request.Cookies.TryGetValue("_ASA", out string cookieValue))
                    {
                        var sessions = await context.Sessions.FirstOrDefaultAsync(u => u.KeyHash == cookieValue);

                        var id = sessions.UserId;
                        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
                        if (user != null)
                        {
                            foreach (var post in UsersFollowers)
                            {
                                post.YouFollower = user.Subscribers.Contains(post.Id);
                            }
                        }
                    }

                    return Ok(new { Users = UsersFollowers });

                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPut("Subscribers")]
        public async Task<IActionResult> Subscribers([FromQuery] string user_name)
        {
            try
            {
                if (!Request.Cookies.TryGetValue("_ASA", out string cookieValue))
                {
                    return NotFound();
                }

                var sessions = await context.Sessions
                    .FirstOrDefaultAsync(u => u.KeyHash == cookieValue);

                var id = sessions.UserId;
                var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == user_name);
                var You = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

                if (user != null && You != null)
                {
                    user.Followers.Add(id);
                    You.Subscribers.Add(user.Id);

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
        public async Task<IActionResult> DeleteSubscribers([FromQuery] string user_name)
        {
            try
            {
                if (!Request.Cookies.TryGetValue("_ASA", out string cookieValue))
                {
                    return NotFound();
                }

                var sessions = await context.Sessions
                    .FirstOrDefaultAsync(u => u.KeyHash == cookieValue);

                var id = sessions.UserId;
                var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == user_name);
                var You = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

                if (user != null && You != null)
                {
                    user.Followers.Remove(id);
                    You.Subscribers.Remove(user.Id);

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
                var user = context.Users.FirstOrDefault(u => u.UserName == Account.NickName);

                if (user != null)
                {
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
