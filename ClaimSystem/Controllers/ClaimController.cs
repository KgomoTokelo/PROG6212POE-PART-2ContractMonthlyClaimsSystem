using Microsoft.AspNetCore.Mvc;

namespace ClaimSystem.Controllers
{
    public class ClaimController : Controller
    {
        //this is for Claim razor page
        public IActionResult Claim()
        {
            return View();
        }

        public IActionResult CreateClaim()
        {
            return View();
        }
    }
}
