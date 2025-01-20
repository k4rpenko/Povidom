using KafkaLibrary.Producers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using NoSQL;
using PGAdminDAL;
using PGAdminDAL.Model;
using UserServer.Interface.Hash;
using UserServer.Models.MessageChat;
using UserServer.Models.Post;
using UserServer.Models.Users;

namespace UserServer.Controllers
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

        [HttpGet("chat/{nick}")]
        public async Task<IActionResult> GetUsers(string nick)
        {
            try
            {
                if (!Request.Cookies.TryGetValue("authToken", out string cookieValue))
                {
                    return Unauthorized();
                }
                    

                var id = _jwt.GetUserIdFromToken(cookieValue);
                if (id == null) { return Unauthorized(); }


                var users = await context.User
                    .Where(u => (u.UserName.ToLower().Contains(nick) && u.Followers.Contains(id)  || u.FirstName.ToLower().Contains(nick) && u.Followers.Contains(id) || u.LastName.ToLower().Contains(nick) && u.Followers.Contains(id) || u.UserName.ToLower().Contains(nick) || u.FirstName.ToLower().Contains(nick) || u.LastName.ToLower().Contains(nick)) && u.Id != id)
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
                    PublicKey = u.PublicKey,
                }).ToList();

                return Ok(new { user = fleetsUsers});
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }


        [HttpGet("{Nick}")]
        public async Task<IActionResult> UserGet(string Nick)
        {
            try
            {
                var user = await context.User.FirstOrDefaultAsync(u => u.UserName == Nick);
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

                    string id = null;
                    UserModel You = null;

                    if (Request.Cookies.TryGetValue("authToken", out string cookieValue))
                    {
                        id = _jwt.GetUserIdFromToken(cookieValue);
                        if (id != null)
                        {
                            You = await context.User.FirstOrDefaultAsync(u => u.Id == id);
                        }
                    }

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
                                    YouRetpost = You != null ? You.RetweetPostID.Contains(post.Id.ToString()) ? true : false : false,
                                    Hashtags = post.Hashtags?.Count ?? 0,
                                    Mentions = post.Mentions?.Count ?? 0,
                                    CommentAmount = post.Comments?.Count ?? 0,
                                    YouComment = You != null ? You.CommentPostID.Contains(post.Id.ToString()) ? true : false : false,
                                    Views = post.Views?.Count ?? 0,
                                    SPublished = post.SPublished
                                });
                            }
                        }
                    }


                    await AddPosts(user.PostID, post => userAccount.Post.Add(post));
                    await AddPosts(user.RetweetPostID, post => userAccount.Post.Add(post));
                    await AddPosts(user.RecallPostId, post => userAccount.RecallPost.Add(post));

                    return Ok(userAccount);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }

        }

        [HttpGet("")]
        public async Task<IActionResult> UserGetToken()
        {
            try
            {
                if (Request.Cookies.TryGetValue("authToken", out string cookieValue))
                {
                    var id = _jwt.GetUserIdFromToken(cookieValue);
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

                    var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
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
                        };

                        
                        if (id != null)
                        {
                            fleetsUser.SubscribersBool = user.Subscribers.Contains(id);
                            fleetsUser.FollowersBool = user.Followers.Contains(id);
                        }
                        return Ok(new { User = fleetsUser });
                    }                    
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Subscribers/{Nick}")]
        public async Task<IActionResult> Subscribers(string Nick, int size)
        {
            try
            {
                var user = await context.User.FirstOrDefaultAsync(u => u.UserName == Nick);
                if (user != null)
                {
                    List<FleetsUserFModel> UsersSubscribers = new List<FleetsUserFModel>();

                    var subscribers = user.Subscribers
                        .Skip(size)
                        .Take(10)
                        .ToList();

                    foreach (var followerId in subscribers)
                    {
                        var userF = await context.User.FirstOrDefaultAsync(u => u.Id == followerId);

                        if (userF != null)
                        {
                            UsersSubscribers.Add(new FleetsUserFModel
                            {
                                Id = userF.Id,
                                UserName = userF.UserName,
                                FirstName = userF.FirstName,
                                LastName = userF.LastName,
                                Avatar = userF.Avatar,
                                Title = userF.Title,
                                PostID = userF.PostID
                            });
                        }
                    }

                    return Ok(new { UsersSubscribers = UsersSubscribers });
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Followers/{Nick}")]
        public async Task<IActionResult> Followers(string Nick, int size)
        {
            try
            {
                var user = await context.User.FirstOrDefaultAsync(u => u.UserName == Nick);
                if (user != null)
                {
                    List<FleetsUserFModel> UsersFollowers = new List<FleetsUserFModel>();

                    var Followers = user.Followers
                        .Skip(size)
                        .Take(10)
                        .ToList();

                    foreach (var followerId in Followers)
                    {
                        var userF = await context.User.FirstOrDefaultAsync(u => u.Id == followerId);

                        if (userF != null) 
                        {
                            UsersFollowers.Add(new FleetsUserFModel
                            {
                                Id = userF.Id,
                                UserName = userF.UserName,
                                FirstName = userF.FirstName,
                                LastName = userF.LastName,
                                Avatar = userF.Avatar,
                                Title = userF.Title,
                                PostID = userF.PostID
                            });
                        }
                    }

                    return Ok(new { UsersFollowers = UsersFollowers });

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
                Console.WriteLine(Account.Id);
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == Account.Id);
                if (!Request.Cookies.TryGetValue("authToken", out string cookieValue))
                {
                    return Unauthorized();
                }

                var id = _jwt.GetUserIdFromToken(cookieValue);
                var You = await context.User.FindAsync(id);

                if (user != null && You != null)
                {
                    user.Followers.Add(id);
                    You.Subscribers.Add(Account.Id);

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
        public async Task<IActionResult> DeleteSubscribers([FromQuery] string NickName, [FromQuery] string userId)
        {
            try
            {
                var user = await context.User.FirstOrDefaultAsync(u => u.UserName == NickName);
                var id = _jwt.GetUserIdFromToken(userId);
                var You = await context.User.FindAsync(id);

                if (user != null && You != null)
                {
                    user.Followers.Remove(You.Id);
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
                var user = context.User.FirstOrDefault(u => u.UserName == Account.NickName);

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
