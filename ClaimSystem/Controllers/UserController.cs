using Microsoft.AspNetCore.Mvc;

namespace ClaimSystem.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
