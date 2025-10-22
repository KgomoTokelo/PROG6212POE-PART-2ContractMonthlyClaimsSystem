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
        public async Task<IActionResult> Decline(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }

            //changes the verifed to rejected for claim status
            claim.Status = Claim.status.Decline;
            _context.Update(claim);
            await _context.SaveChangesAsync();

            // Reload only pending claims for verification
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
        public async Task<IActionResult> Reject(int id)
        {
            //looking for specific claim
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }

            //changes the verifed to rejected for claim status
            claim.Status = Claim.status.Rejected;
            _context.Update(claim);
            await _context.SaveChangesAsync();

            // Reload only pending claims for verification
            var claims = await _context.Claims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == Claim.status.Submitted)
                .ToListAsync();

            return View("Verify", claims);

            //i am loading my enum datatype onto var datatype statuses
            
        }
        
        public async Task<IActionResult> VerificationProcess()
        {

            return View();
        }

    }
    }


