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
                // Prepare dropdown
                var statuses = Enum.GetValues(typeof(Claims.status))
                    .Cast<Claims.status>()
                    .Select(s => new SelectListItem
                    {
                        Text = s.ToString(),
                        Value = s.ToString()
                    }).ToList();
                ViewBag.StatusList = statuses;

                // Fetch all Verified claims
                var verifiedClaims = await _context.Claims
                    .Include(c => c.Lecturer)
                    .Where(c => c.Status == Claims.status.Verefied)
                    .ToListAsync();

                var notifications = new List<string>();

                // Apply automatic approval/rejection based on guidelines
                foreach (var claim in verifiedClaims)
                {
                    bool isStem = claim.Lecturer.Department.ToLower().Contains("stem");
                    int minHours = isStem ? 4 : 3;
                    int maxHours = 8;

                    if (claim.HoursWorked > maxHours)
                    {
                        // Auto-Reject
                        claim.Status = Claims.status.Decline;
                        claim.Comments = $"Auto-rejected: exceeded max hours ({maxHours}).";
                        notifications.Add($"Claim {claim.ClaimID} auto-rejected.");
                    }
                    else if (claim.HoursWorked >= minHours)
                    {
                        // Auto-Approve
                        claim.Status = Claims.status.Approved;
                        claim.Comments = "Auto-approved: meets guideline hours.";
                        notifications.Add($"Claim {claim.ClaimID} auto-approved.");
                    }
                    else
                    {
                        // Still pending: leave as Verefied
                        notifications.Add($"Claim {claim.ClaimID} pending: insufficient hours.");
                    }
                }

                if (verifiedClaims.Any())
                {
                    _context.UpdateRange(verifiedClaims);
                    await _context.SaveChangesAsync();
                }

                TempData["Notifications"] = string.Join("<br/>", notifications);

                // Load claims to show in view (Approved, Declined, Pending)
                var viewClaims = await _context.Claims
                    .Include(c => c.Lecturer)
                    .Where(c => c.Status == Claims.status.Verefied ||
                                c.Status == Claims.status.Approved ||
                                c.Status == Claims.status.Decline)
                    .ToListAsync();

                return View(viewClaims);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error processing claims: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while processing claims.";
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

                claim.Status = Claims.status.Approved;
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

                claim.Status = Claims.status.Decline;
                claim.Comments = comments;

                _context.Update(claim);
                await _context.SaveChangesAsync();

                var claims = await _context.Claims
                    .Include(c => c.Lecturer)
                    .Where(c => c.Status == Claims.status.Verefied)
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
                var submittedClaims = await _context.Claims
                    .Include(c => c.Lecturer)
                    .Where(c => c.Status == Claims.status.Submitted)
                    .ToListAsync();

                foreach (var claim in submittedClaims)
                {
                    bool isStem = claim.Lecturer.Department.ToLower().Contains("stem");
                    int expectedRate = isStem ? 98 : 88;

                    // RULE 1: Check hourly rate
                    if (claim.HourlyRate != expectedRate)
                    {
                        claim.Status = Claims.status.Rejected;
                        claim.Comments = $"Incorrect hourly rate. Expected {expectedRate}, got {claim.HourlyRate}.";
                        continue;
                    }

                    // RULE 2: Check hours worked
                    if (claim.HoursWorked > 8)
                    {
                        claim.Status = Claims.status.Rejected;
                        claim.Comments = "Hours exceed maximum allowed limit of 8 hours.";
                        continue;
                    }

                    // All rules passed Auto verify
                    claim.Status = Claims.status.Verefied;
                    claim.Comments = "Automatically verified according to guidelines.";
                }

                await _context.SaveChangesAsync();

                // Now return full claim list for the dashboard
                var allClaims = await _context.Claims.Include(c => c.Lecturer).ToListAsync();
                return View(allClaims);
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
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
                return NotFound();

            claim.Status = Claims.status.Verefied;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Claim manually verified.";
            return RedirectToAction("Verify");
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
                claim.Status = Claims.status.Rejected;
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
                    .Where(c => c.Status == Claims.status.Submitted)
                    .ToListAsync();

                TempData["InfoMessage"] = "Claim rejected successfully.";
                return View("Verify", claims);
            }
            catch (DbUpdateException dbEx)
            {
                Console.Error.WriteLine($"Database error rejecting claim: {dbEx.Message}");
                var errorModel = new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                };
                ViewBag.ErrorMessage = "A database error occurred while rejecting the claim.";
                return View("Error", errorModel);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unexpected error rejecting claim: {ex.Message}");
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


