using Microsoft.AspNetCore.Mvc;

namespace ClaimSystem.Controllers
{
    public class UploadDocumentsController : Controller
    {
        public IActionResult Upload()
        {
            return View();
        }
    }
}
