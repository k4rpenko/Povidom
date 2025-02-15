using Microsoft.AspNetCore.Mvc;

namespace message.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class test : Controller
    {
        [HttpPost("Test")]
        public async Task<IActionResult> Test()
        {
            return Ok();
        }
    }
}
