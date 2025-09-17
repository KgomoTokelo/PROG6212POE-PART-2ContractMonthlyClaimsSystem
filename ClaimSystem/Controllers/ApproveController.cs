using Microsoft.AspNetCore.Mvc;

namespace ClaimSystem.Controllers
{
    public class ApproveController : Controller
    {
        public IActionResult Approve()
        {
            return View();
        }

        public IActionResult Verify()
        {
            return View();
        }
    }
}
