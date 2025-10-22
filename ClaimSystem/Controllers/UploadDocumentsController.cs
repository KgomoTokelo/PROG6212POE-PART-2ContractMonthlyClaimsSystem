using ClaimSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClaimSystem.Controllers
{
    public class UploadDocumentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public UploadDocumentsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: UploadDocuments/Upload
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(UploadDocuments model, IFormFile fileUpload)
        {
            try
            {
                if (fileUpload == null || fileUpload.Length == 0)
                {
                    ModelState.AddModelError("fileUpload", "Please select a file to upload.");
                    return View(model);
                }

                // Allow only specific file extensions
                var allowedExtensions = new[] { ".pdf", ".docx", ".jpg", ".png" };
                var extension = Path.GetExtension(fileUpload.FileName).ToLower();

                if (Array.IndexOf(allowedExtensions, extension) < 0)
                {
                    ModelState.AddModelError("fileUpload", "Unsupported file format.");
                    return View(model);
                }

                // Create upload folder if not exists
                var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                // Create unique file name
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadFolder, uniqueFileName);

                // Save file to wwwroot/uploads
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileUpload.CopyToAsync(stream);
                }

                // Save file info to DB
                model.FileName = fileUpload.FileName;
                model.FilePath = "/uploads/" + uniqueFileName;
                model.UploadDate = DateTime.Now;

                _context.UploadDocuments.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "File uploaded successfully!";
                Console.WriteLine("Successful upload");

                return RedirectToAction("UploadSuccess");
            }
            catch (IOException ioEx)
            {
                // Handle file-related errors
                Console.Error.WriteLine($"File error: {ioEx.Message}");
                ModelState.AddModelError("", "An error occurred while saving the file. Please try again.");
                return View(model);
            }
            catch (Exception ex)
            {
                // Handle general errors
                Console.Error.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "Unexpected error occurred while uploading the file.");
                return View(model);
            }
        }

        // GET: UploadDocuments/UploadSuccess
        public IActionResult UploadSuccess()
        {
            try
            {
                if (TempData["SuccessMessage"] == null)
                {
                    ViewBag.Message = "File uploaded successfully!";
                }
                else
                {
                    ViewBag.Message = TempData["SuccessMessage"];
                }

                return View();
            }
            catch (Exception ex)
            {
                // Log or handle unexpected rendering issues
                Console.Error.WriteLine($"Error displaying success page: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while loading the success page.";
                return View("Error"); // Make sure you have an Error.cshtml view
            }
        }
    }
}
