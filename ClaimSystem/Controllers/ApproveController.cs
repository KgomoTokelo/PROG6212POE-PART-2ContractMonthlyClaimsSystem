using Microsoft.AspNetCore.Mvc;

namespace ClaimSystem.Controllers
{
    public class ApproveController : Controller
    {
        //this is for Approve razor page
        public IActionResult Approve()
        {
            return View();
        }
        //this is for verify razor page
        public IActionResult Verify()
        {
            return View();
        }
    }
}
