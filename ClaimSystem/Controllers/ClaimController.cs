using ClaimSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClaimSystem.Controllers
{
    public class ClaimController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClaimController(ApplicationDbContext context)
        {
            _context = context;
        }

        // View all claims for logged-in lecturer
        public async Task<IActionResult> Claim()
        {
            try
            {
                // Get logged-in Identity user GUID
                var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(identityUserId))
                {
                    ViewBag.ErrorMessage = "User not logged in.";
                    return View("Error");
                }

                // Find local Users record
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);

                if (user == null)
                {
                    ViewBag.ErrorMessage = "User profile not found.";
                    return View("Error");
                }

                // Find lecturer record linked to this user
                var lecturer = await _context.Lecturers
                    .FirstOrDefaultAsync(l => l.UsersId == user.Id);

                if (lecturer == null)
                {
                    ViewBag.ErrorMessage = "Lecturer profile not found.";
                    return View("Error");
                }

                // Fetch claims for this lecturer
                var claims = await _context.Claims
                    .Where(c => c.LecturerID == lecturer.LecturerID)
                    .Include(c => c.Lecturer)
                    .Include(c => c.Documents)
                    .ToListAsync();

                return View(claims);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading claims: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while loading your claims.";
                return View("Error");
            }
        }

        // CreateClaim
        [HttpGet]
        public async Task<IActionResult> CreateClaim()
        {
            try
            {
                ViewBag.StatusList = Enum.GetValues(typeof(Claims.status))
                    .Cast<Claims.status>()
                    .Select(s => new SelectListItem { Text = s.ToString(), Value = s.ToString() })
                    .ToList();

                var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var user = await _context.Users.FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "User profile not found.";
                    return View("Error");
                }

                var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.UsersId == user.Id);
                if (lecturer == null)
                {
                    ViewBag.ErrorMessage = "Lecturer profile not found.";
                    return View("Error");
                }

                var model = new Claims
                {
                    LecturerID = lecturer.LecturerID,
                    Status = Claims.status.Submitted,
                    Lecturer = lecturer
                
                    
                };

                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error preparing CreateClaim view: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while preparing the claim form.";
                return View("Error");
            }
        }

        //  CreateClaim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClaim(Claims model)
        {
            try
            {
                var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "User profile not found.";
                    return View("Error");
                }

                var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.UsersId == user.Id);
                if (lecturer == null)
                {
                    ViewBag.ErrorMessage = "Lecturer profile not found.";
                    return View("Error");
                }

                model.LecturerID = lecturer.LecturerID;

                if (ModelState.IsValid)
                {
                    _context.Claims.Add(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Claim submitted successfully!";
                    return RedirectToAction("Claim");
                }

                ViewBag.StatusList = Enum.GetValues(typeof(Claims.status))
                    .Cast<Claims.status>()
                    .Select(s => new SelectListItem { Text = s.ToString(), Value = s.ToString() })
                    .ToList();

                return View(model);
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database error: {dbEx.Message}");
                ViewBag.ErrorMessage = "A database error occurred while saving your claim.";
                return View("Error");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                ViewBag.ErrorMessage = "An unexpected error occurred while submitting your claim.";
                return View("Error");
            }
        }

        // View individual claim
        public async Task<IActionResult> Views(int id)
        {
            try
            {
                if (id == 0) return NotFound();

                var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "User profile not found.";
                    return View("Error");
                }

                var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.UsersId == user.Id);
                if (lecturer == null)
                {
                    ViewBag.ErrorMessage = "Lecturer profile not found.";
                    return View("Error");
                }

                var claim = await _context.Claims
                    .Include(c => c.Lecturer)
                    .Include(c => c.Documents)
                    .FirstOrDefaultAsync(c => c.ClaimID == id && c.LecturerID == lecturer.LecturerID);

                if (claim == null) return NotFound();

                return View(claim);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading claim details: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while loading the claim details.";
                return View("Error");
            }
        }
    }
}
