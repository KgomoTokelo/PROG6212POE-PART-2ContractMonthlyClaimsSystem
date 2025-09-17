using Microsoft.AspNetCore.Mvc;

namespace ClaimSystem.Controllers
{
    public class LecturerController : Controller
    {
        //this is for Lecturer model
        public IActionResult Index()
        {
            return View();
        }
    }
}
