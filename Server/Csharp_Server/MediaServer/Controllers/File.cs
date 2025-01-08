using Microsoft.AspNetCore.Mvc;

namespace MediaServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class File : Controller
    {
        [HttpGet("GetFile")]
        public IActionResult GetFile()
        {
            return Ok();
        }
    }
}
