using Server.Hash;
using Server.Sending;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoogleAuth : Controller
    {
        GoogleOAuth GoogleOAuth = new GoogleOAuth();

        [HttpGet("GoogleAuth")]
        public async Task<IActionResult> RedirectOauthServer()
        {
            var scope = "https://mail.google.com/";
            var redirectUrl = $"{Request.Scheme}://{Request.Host}/api/GoogleAuthGetCode";
            var CodeVerifier = Guid.NewGuid().ToString();
            var key = new HASH().GenerateKey();
            var codeChellange = new HASH().Encrypt(CodeVerifier, key.ToString());
            HttpContext.Session.SetString("CodeVerifier", CodeVerifier);

            var url = GoogleOAuth.GenerateOauthUrl(scope, redirectUrl, codeChellange);
            return Redirect(url);
        }

        [HttpPost("GoogleAuthGetCode")]
        public async Task<IActionResult> codeOauthServer(string code)
        {
            string CodeVerifier = HttpContext.Session.GetString("CodeVerifier");
            var redirectUrl = $"{Request.Scheme}://{Request.Host}/api/GoogleAuthGetCode";

            var token = await GoogleOAuth.ExechangeCodeOauthT(code, CodeVerifier, redirectUrl);
            return Ok();
        }
    }
}