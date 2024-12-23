using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQL;
using PGAdminDAL;
using PGAdminDAL.Model;
using Server.Models.MessageChat;
using Server.Models.Post;
using Server.Models.Users;
using Server.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Fleets : Controller
    {
        private readonly IMongoCollection<SpacePostModel> _customers;
        private readonly AppDbContext context;
        private readonly JWT _jwt = new JWT();

        public Fleets(AppDbContext _context, AppMongoContext _Mongo,  IConfiguration _configuration) { context = _context; _customers = _Mongo.Database?.GetCollection<SpacePostModel>(_configuration.GetSection("MongoDB:MongoDbDatabase").Value); }

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
                    

                var id = new JWT().GetUserIdFromToken(cookieValue);
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
                    var PostForID = new List<SpacePostModel>();
                    var Recall = new List<SpacePostModel>();
                    foreach (var item in user.PostID)
                    {
                        var objectId = ObjectId.Parse(item);
                        var post = await _customers.Find(post => post.Id == objectId).FirstOrDefaultAsync();
                        PostForID.Add(post);
                    }
                    foreach (var item in user.RetweetPostID)
                    {
                        var objectId = ObjectId.Parse(item);
                        var post = await _customers.Find(post => post.Id == objectId).FirstOrDefaultAsync();
                        PostForID.Add(post);
                    }

                    foreach (var item in user.RecallPostId)
                    {
                        var objectId = ObjectId.Parse(item);
                        var post = await _customers.Find(post => post.Id == objectId).FirstOrDefaultAsync();
                        Recall.Add(post);
                    }

                    var userAccount = new UserAccount
                    {
                        Id = user.Id.ToString(),
                        Avatar = user.Avatar,
                        UserName = user.UserName,
                        Title = user.Title,
                        PhoneNumber = user.PhoneNumber,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Post = PostForID,
                        RecallPost = Recall,
                        FollowersAmount = user.Followers.Count,
                        SubscribersAmount = user.Subscribers.Count,
                    };

                    if (Request.Cookies.TryGetValue("Token", out string cookieValue))
                    {
                        var id = new JWT().GetUserIdFromToken(cookieValue);
                        if (id != null)
                        {
                            userAccount.YouSubscriber = user.Subscribers.Contains(id);
                            userAccount.YouFollower = user.Followers.Contains(id);
                        }
                    }

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
                    var id = new JWT().GetUserIdFromToken(cookieValue);
                    var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
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
                            PostID = user.PostID
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

                var id = new JWT().GetUserIdFromToken(cookieValue);
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
        public async Task<IActionResult> DeleteSubscribers(AccountSettingsModel Account)
        {
            try
            {
                var user = await context.User.FirstOrDefaultAsync(u => u.UserName == Account.NickName);
                var id = _jwt.GetUserIdFromToken(Account.Token);
                var You = await context.User.FindAsync(id);

                if (user != null && You != null)
                {
                    user.Followers.Remove(Account.Id);
                    You.Subscribers.Remove(Account.NickName);

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
