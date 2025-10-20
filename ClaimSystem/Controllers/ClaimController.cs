using ClaimSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClaimSystem.Models;


namespace ClaimSystem.Controllers
{
    public class ClaimController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClaimController(ApplicationDbContext context)
        {
            _context = context;
        }

        //this is for Claim razor page
        public async Task<IActionResult> Claim()
        {
            var claims = await _context.Claims.Include(c => c.Lecturer).ToListAsync();

            return View(claims);
          
        }
        [HttpGet]
        public IActionResult CreateClaim()
        {

            var statuses = Enum.GetValues(typeof(Claim.status))
                           .Cast<Claim.status>()
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

            return View(new Claim());
        }

        //creating the method to send user input from razor view Createclaim
        [HttpPost]//a post method sends to server in basic terms
        public async Task<IActionResult> CreateClaim(Claim model)
        {
            if (ModelState.IsValid)
            {
                //saves to database
                _context.Claims.Add(model);
               await _context.SaveChangesAsync();
                Console.WriteLine("✅ Claim saved successfully.");
                return RedirectToAction("Claim");
            }

            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Field: {state.Key}, Error: {error.ErrorMessage}");
                }
            }

            ViewBag.StatusList = Enum.GetValues(typeof(Claim.status))
                              .Cast<Claim.status>()
                              .Select(s => new SelectListItem
                              {
                                  Text = s.ToString(),
                                  Value = s.ToString()
                              }).ToList();
            Console.WriteLine("✅ Claim saved unsuccessfully.");

            return View(model);
        }
    }
}
