using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
