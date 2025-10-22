using ClaimSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;



namespace ClaimSystem.Controllers
{
    public class ApproveController : Controller
    {


         private readonly ApplicationDbContext _context;

    public ApproveController(ApplicationDbContext context)
    {
        _context = context;
    }
    //this is for Approve razor page
    public async Task<IActionResult> Approve()
        {
            //i am loading my enum datatype onto var datatype statuses
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

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            //if no claim then returns notfound
            if (claim == null)
            {
                return NotFound();
            }

            // Update status to Verified
            claim.Status = Claim.status.Approved;
            _context.Update(claim);
            await _context.SaveChangesAsync();

            // Redirect to Approve page
            return RedirectToAction("Approve");
            
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Decline(int id, string comments)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
                return NotFound();

            claim.Status = Claim.status.Decline;
            claim.Comments = comments; // save the comment
            _context.Update(claim);
            await _context.SaveChangesAsync();

            // reload pending claims
            var claims = await _context.Claims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == Claim.status.Verefied)
                .ToListAsync();

            return View("Approve", claims);
        }


        //this is for verify razor page
        public async Task<IActionResult> Verify()
        {
            //i am loading my enum datatype onto var datatype statuses
            var claims = await _context.Claims.Include(c => c.Lecturer)
        .Where(c => c.Status == Claim.status.Submitted) // only show claims that need verification
        .ToListAsync();

            return View(claims);
        }

        [HttpPost]
        public async Task<IActionResult> Verify(int id)
        {
            //checks for claim in database
            var claim = await _context.Claims.FindAsync(id);
            //if no claim then returns notfound
            if (claim == null)
            {
                return NotFound();
            }

            // Update status to Verified
            claim.Status = Claim.status.Verefied;
            _context.Update(claim);
            await _context.SaveChangesAsync();

            // Redirect to Approve page
            return RedirectToAction("Verify");
            
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id, string comments)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }

            // Update claim status
            claim.Status = Claim.status.Rejected;
            _context.Update(claim);

            // Record rejection in Approve table
            var approvalRecord = new Approve
            {
                ClaimID = id,
                UserID = 1, // ← Replace this with logged-in user’s ID if you have authentication
                ApprovalDate = DateTime.Now,
                Decision = "Rejected",
                Comments = comments
            };

            _context.Approves.Add(approvalRecord);
            await _context.SaveChangesAsync();

            // Reload only pending claims
            var claims = await _context.Claims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == Claim.status.Submitted)
                .ToListAsync();

            return View("Verify", claims);
        }


        public async Task<IActionResult> VerificationProcess()
        {

            return View();
        }

    }
    }


