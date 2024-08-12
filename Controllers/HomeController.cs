using Microsoft.AspNetCore.Mvc;

namespace G_IPG_API.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("Home/Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
