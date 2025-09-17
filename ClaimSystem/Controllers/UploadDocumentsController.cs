using Microsoft.AspNetCore.Mvc;

namespace ClaimSystem.Controllers
{
    public class UploadDocumentsController : Controller
    {
        //this is for Upload razor page
        public IActionResult Upload()
        {
            return View();
        }
    }
}
