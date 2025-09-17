using Microsoft.AspNetCore.Mvc;

namespace ClaimSystem.Controllers
{
    public class ClaimController : Controller
    {
        public IActionResult Claim()
        {
            return View();
        }
    }
}
