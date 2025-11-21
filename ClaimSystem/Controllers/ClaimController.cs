using ClaimSystem.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClaimSystem.Controllers
{
    public class ClaimController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ClaimController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        public async Task<IActionResult> Claim()
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);
            if (user == null) return View("Error");

            var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.UsersId == user.Id);
            if (lecturer == null) return View("Error");

            var claims = await _context.Claims
                .Where(c => c.LecturerID == lecturer.LecturerID)
                .Include(c => c.Documents)
                .ToListAsync();

            return View(claims);
        }

        
        [HttpGet]
        public async Task<IActionResult> CreateClaim()
        {
            ViewBag.StatusList = Enum.GetValues(typeof(Claims.status))
                .Cast<Claims.status>()
                .Select(s => new SelectListItem { Text = s.ToString(), Value = s.ToString() })
                .ToList();

            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);
            if (user == null) return View("Error");

            var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.UsersId == user.Id);
            if (lecturer == null) return View("Error");

            var model = new Claims
            {
                LecturerID = lecturer.LecturerID,
                Lecturer = lecturer,
                Status = Claims.status.Submitted,
                ModuleName = lecturer.Department,      
                HourlyRate = lecturer.DefaultRatePerJob,
                SubmissionDate = DateTime.Now
            };

            return View(model);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClaim(Claims model, IFormFile fileUpload)
        {
            ViewBag.StatusList = Enum.GetValues(typeof(Claims.status))
                .Cast<Claims.status>()
                .Select(s => new SelectListItem { Text = s.ToString(), Value = s.ToString() })
                .ToList();

            if (!ModelState.IsValid)
                return View(model);

            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);
            if (user == null) return View("Error");

            var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.UsersId == user.Id);
            if (lecturer == null) return View("Error");

            model.LecturerID = lecturer.LecturerID;

            // Save claim
            _context.Claims.Add(model);
            await _context.SaveChangesAsync();

           
            if (fileUpload != null && fileUpload.Length > 0)
            {
                string[] allowedExtensions = { ".pdf", ".docx", ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(fileUpload.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("", "Invalid file format.");
                    return View(model);
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = $"{Guid.NewGuid()}{extension}";
                string fullPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await fileUpload.CopyToAsync(stream);
                }

                var doc = new UploadDocuments
                {
                    ClaimID = model.ClaimID,
                    FileName = fileUpload.FileName,
                    FilePath = "/uploads/" + uniqueFileName,
                    UploadDate = DateTime.Now
                };

                _context.UploadDocuments.Add(doc);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Claim created successfully!";
            return RedirectToAction("Claim");
        }

        
        public async Task<IActionResult> Views(int id)
        {
            var claim = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.ClaimID == id);

            if (claim == null)
                return NotFound();

            return View(claim);
        }
    }
}
