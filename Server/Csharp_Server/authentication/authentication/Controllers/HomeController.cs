using Microsoft.AspNetCore.Mvc;

namespace authentication.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
