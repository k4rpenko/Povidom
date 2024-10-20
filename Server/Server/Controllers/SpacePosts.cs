using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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

        public SpacePosts(AppMongoContext _Mongo, IConfiguration _configuration) { _customers = _Mongo.Database?.GetCollection<SpacePostModel>(_configuration.GetSection("MongoDB:MongoDbDatabase").Value); }

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
    }
}
