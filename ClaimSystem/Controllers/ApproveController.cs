using ClaimSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ClaimSystem.Controllers
{
    public class ApproveController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApproveController(ApplicationDbContext context)
        {
            _context = context;
        }

        // a method that fetches for approve from verfiy page
        public async Task<IActionResult> Approve()
        {
            try
            {
                var statuses = Enum.GetValues(typeof(Claim.status))
                    .Cast<Claim.status>()
                    .Select(s => new SelectListItem
                    {
                        Text = s.ToString(),
                        Value = s.ToString()
                    }).ToList();

                ViewBag.StatusList = statuses;

                var claims = await _context.Claims
                    .Include(c => c.Lecturer)
                    .Where(c => c.Status == Claim.status.Verefied)
                    .ToListAsync();

                return View(claims);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($" Error loading Approve page: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while loading the approval list.";
                return View("Error");
            }
        }

        //a method for updating database for approved lecturese
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var claim = await _context.Claims.FindAsync(id);
                if (claim == null)
                    return NotFound();

                claim.Status = Claim.status.Approved;
                _context.Update(claim);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Claim approved successfully!";
                return RedirectToAction("Approve");
            }
            catch (DbUpdateException dbEx)
            {
                Console.Error.WriteLine($"Database error approving claim: {dbEx.Message}");
                ViewBag.ErrorMessage = "A database error occurred while approving the claim.";
                return View("Error");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unexpected error approving claim: {ex.Message}");
                ViewBag.ErrorMessage = "An unexpected error occurred while approving the claim.";
                return View("Error");
            }
        }

        //a method for updating database for declined lectures
        [HttpPost]
        public async Task<IActionResult> Decline(int id, string comments)
        {
            try
            {
                var claim = await _context.Claims.FindAsync(id);
                if (claim == null)
                    return NotFound();

                claim.Status = Claim.status.Decline;
                claim.Comments = comments;

                _context.Update(claim);
                await _context.SaveChangesAsync();

                var claims = await _context.Claims
                    .Include(c => c.Lecturer)
                    .Where(c => c.Status == Claim.status.Verefied)
                    .ToListAsync();

                TempData["InfoMessage"] = "Claim declined successfully.";
                return View("Approve", claims);
            }
            catch (DbUpdateException dbEx)
            {
                Console.Error.WriteLine($" Database error declining claim: {dbEx.Message}");
                ViewBag.ErrorMessage = "A database error occurred while declining the claim.";
                return View("Error");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($" Unexpected error declining claim: {ex.Message}");
                ViewBag.ErrorMessage = "An unexpected error occurred while declining the claim.";
                return View("Error");
            }
        }

        // a method that fecthes info from databse to see lectures that need to be approved
        public async Task<IActionResult> Verify()
        {
            try
            {
                var claims = await _context.Claims
                    .Include(c => c.Lecturer)
                    .Where(c => c.Status == Claim.status.Submitted)
                    .ToListAsync();

                return View(claims);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error loading Verify page: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while loading claims for verification.";
                return View("Error");
            }
        }

        //a method that updates database for lecture status verified
        [HttpPost]
        public async Task<IActionResult> Verify(int id)
        {
            try
            {
                var claim = await _context.Claims.FindAsync(id);
                if (claim == null)
                    return NotFound();

                claim.Status = Claim.status.Verefied;
                _context.Update(claim);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Claim verified successfully!";
                return RedirectToAction("Verify");
            }
            catch (DbUpdateException dbEx)
            {
                Console.Error.WriteLine($"Database error verifying claim: {dbEx.Message}");
                ViewBag.ErrorMessage = "A database error occurred while verifying the claim.";
                return View("Error");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"unexpected error verifying claim: {ex.Message}");
                ViewBag.ErrorMessage = "An unexpected error occurred while verifying the claim.";
                return View("Error");
            }
        }


        //a method that updates lectures that are rejected to database
        [HttpPost]
        public async Task<IActionResult> Reject(int id, string comments)
        {
            try
            {
                // Find claim
                var claim = await _context.Claims.FindAsync(id);
                if (claim == null)
                {
                    TempData["ErrorMessage"] = $"No claim found with ID {id}.";
                    return RedirectToAction("Verify", "Claim");
                }

                // Update claim status
                claim.Status = Claim.status.Rejected;
                _context.Update(claim);

                // Create approval record
                var approvalRecord = new Approve
                {
                    ClaimID = id,
                    UserID = 1, // TODO: Replace with logged-in user ID
                    ApprovalDate = DateTime.Now,
                    Decision = "Rejected",
                    Comments = string.IsNullOrWhiteSpace(comments) ? "No comment provided" : comments
                };

                _context.Approves.Add(approvalRecord);
                await _context.SaveChangesAsync();

                // Reload claims still needing verification
                var claims = await _context.Claims
                    .Include(c => c.Lecturer)
                    .Where(c => c.Status == Claim.status.Submitted)
                    .ToListAsync();

                TempData["InfoMessage"] = "Claim rejected successfully.";
                return View("Verify", claims);
            }
            catch (DbUpdateException dbEx)
            {
                Console.Error.WriteLine($"❌ Database error rejecting claim: {dbEx.Message}");
                var errorModel = new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                };
                ViewBag.ErrorMessage = "A database error occurred while rejecting the claim.";
                return View("Error", errorModel);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Unexpected error rejecting claim: {ex.Message}");
                var errorModel = new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                };
                ViewBag.ErrorMessage = "An unexpected error occurred while rejecting the claim.";
                return View("Error", errorModel);
            }
        }


        public IActionResult VerificationProcess()
        {
           
                return View("Error");
            }
        }
    }


