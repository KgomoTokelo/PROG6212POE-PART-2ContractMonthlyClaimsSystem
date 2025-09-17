using Microsoft.AspNetCore.Mvc;

namespace ClaimSystem.Controllers
{
    public class UserController : Controller
    {
        //this is for User model
        public IActionResult Index()
        {
            return View();
        }
    }
}
