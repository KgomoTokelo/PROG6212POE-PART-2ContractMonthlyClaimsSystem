using ClaimSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClaimSystem.Controllers
{
    public class ClaimController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClaimController(ApplicationDbContext context)
        {
            _context = context;
        }

        // for claim page to view all clams
        public async Task<IActionResult> Claim()
        {
            try
            {
                var claims = await _context.Claims.Include(c => c.Lecturer).ToListAsync();
                return View(claims);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error loading claims: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while loading claims. Please try again later.";
                return View("Error");
            }
        }

        // a method that fetches data from database for lectures and status
        [HttpGet]
        public IActionResult CreateClaim()
        {
            try
            {
                var statuses = Enum.GetValues(typeof(Claims.status))
                    .Cast<Claims.status>()
                    .Select(s => new SelectListItem
                    {
                        Text = s.ToString(),
                        Value = s.ToString()
                    }).ToList();

                ViewBag.StatusList = statuses;

                var lecturers = _context.Lecturers
                    .Select(l => new SelectListItem
                    {
                        Value = l.LecturerID.ToString(),
                        Text = l.Name
                    }).ToList();

                ViewBag.LecturerList = lecturers;

                return View(new Claims());
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error preparing CreateClaim view: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while preparing the claim creation form.";
                return View("Error");
            }
        }

        //  a post method for creating views esstentially adds or updates database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClaim(Claims model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Claims.Add(model);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Claim saved successfully.");
                    TempData["SuccessMessage"] = "Claim submitted successfully!";
                    return RedirectToAction("Claim");
                }

                // for logging errors
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Field: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }

                // populates dropdowns for view
                ViewBag.StatusList = Enum.GetValues(typeof(Claims.status))
                    .Cast<Claims.status>()
                    .Select(s => new SelectListItem
                    {
                        Text = s.ToString(),
                        Value = s.ToString()
                    }).ToList();

                ViewBag.LecturerList = _context.Lecturers
                    .Select(l => new SelectListItem
                    {
                        Value = l.LecturerID.ToString(),
                        Text = l.Name
                    }).ToList();

                Console.WriteLine(" Claim validation failed.");
                return View(model);
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($" Database error while saving claim: {dbEx.Message}");
                ViewBag.ErrorMessage = "A database error occurred while saving your claim. Please try again.";
                return View("Error");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Unexpected error while saving claim: {ex.Message}");
                ViewBag.ErrorMessage = "An unexpected error occurred while submitting your claim.";
                return View("Error");
            }
        }

        // a method to view indivual claims
        public async Task<IActionResult> Views(int id)
        {
            try
            {
                if (id == 0)
                {
                    return NotFound();
                }

                var claim = await _context.Claims
                    .Include(c => c.Lecturer)
                    .Include(c => c.Documents)
                    .FirstOrDefaultAsync(c => c.ClaimID == id);

                if (claim == null)
                    return NotFound();

                return View(claim);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error loading claim details: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while loading the claim details.";
                return View("Error");
            }
        }
    }
}
