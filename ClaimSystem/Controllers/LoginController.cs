using ABCRetailers.Models.ViewModels;
using ClaimSystem.Models;
using ClaimSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ABCRetailers.Controllers
{
    [Authorize(Roles = "HumanResource")]
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;


        public LoginController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {

            _context = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }
        public IActionResult HrDashBoard()
        {
            return View("~/Views/HumanResource/HrDashBoard.cshtml");

        }

        public async Task<IActionResult> EmployeeList()
        {

            var employees = await _context.Users
                .ToListAsync();


            return View("~/Views/HumanResource/EmployeeList.cshtml", employees);
        }


        public IActionResult AddEmployee()
        {
            return View("~/Views/HumanResource/AddEmployee.cshtml", new RegisterViewModel());
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmployee(RegisterViewModel model)
        {
            var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);

            if (!roleExists)
            {
                ModelState.AddModelError("", $"Role '{model.RoleName}' does not exist.");
            }

            if (ModelState.IsValid)
            {
                // 1. Create Identity user
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user, model.TempPassword);

                if (createResult.Succeeded)
                {
                    // 2. Add role
                    await _userManager.AddToRoleAsync(user, model.RoleName);

                    // 3. Add profile in Users table
                    var profile = new Users
                    {
                        IdentityUserId = user.Id,
                        Name = model.Name,
                        Surname = model.Surname,
                        Department = model.Department,
                        Email = model.Email,
                        DefaultRatePerJob = model.DefaultRatePerJob,
                        RoleName = model.RoleName,
                        Password = model.TempPassword
                    };

                    _context.Users.Add(profile);
                    await _context.SaveChangesAsync();

                    //  ADD THIS: If user is Lecturer → insert into Lecturer table
                    if (model.RoleName == "Lecturer")
                    {
                        var lecturer = new Lecturer
                        {
                            UsersId = profile.Id,
                            Name = model.Name,            // required
                            Surname = model.Surname,      // required
                            Email = model.Email,          // required
                            Department = model.Department,
                            DefaultRatePerJob = model.DefaultRatePerJob

                        };

                        _context.Lecturers.Add(lecturer);
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(EmployeeList));
                }

                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"{state.Key}: {error.ErrorMessage}");
                    }
                }
            }

            return View("~/Views/HumanResource/AddEmployee.cshtml", model);
        }



        [Authorize(Roles = "HumanResource")]
        public async Task<IActionResult> HrSummary()
        {
            var claims = await _context.Claims.ToListAsync();

            var totalCount = claims.Count;

            // to calculate totalamount 
            var totalAmount = claims.Sum(c => c.HoursWorked * c.HourlyRate);

            var submittedCount = claims.Count(c => c.Status == Claims.status.Submitted);
            var verifiedCount = claims.Count(c => c.Status == Claims.status.Verefied);
            var approvedCount = claims.Count(c => c.Status == Claims.status.Approved);
            var rejectedCount = claims.Count(c => c.Status == Claims.status.Rejected);
            var declinedCount = claims.Count(c => c.Status == Claims.status.Decline);
            var settledCount = claims.Count(c => c.Status == Claims.status.Settled);

            ViewBag.TotalCount = totalCount;
            ViewBag.TotalAmount = totalAmount;
            ViewBag.SubmittedCount = submittedCount;
            ViewBag.VerifiedCount = verifiedCount;
            ViewBag.ApprovedCount = approvedCount;
            ViewBag.RejectedCount = rejectedCount;
            ViewBag.DeclinedCount = declinedCount;
            ViewBag.SettledCount = settledCount;

            return View("~/Views/HumanResource/HrSummary.cshtml");
        }


        [Authorize(Roles = "HumanResource")]
        public async Task<FileResult> HrExportCsv()
        {
            var claims = await _context.Claims
                .OrderBy(c => c.ClaimID)
                .Include(c => c.Lecturer)   // optional: export lecturer names
                .ToListAsync();

            var lines = new List<string>();
            lines.Add("ClaimID,LecturerID,LecturerName,ModuleName,SubmissionDate,HoursWorked,HourlyRate,TotalAmount,Status,Comments");

            foreach (var c in claims)
            {
                var total = c.HoursWorked * c.HourlyRate;

                var line =
                    $"{c.ClaimID}," +
                    $"{c.LecturerID}," +
                    $"{c.Lecturer?.Name}," +
                    $"{c.ModuleName}," +
                    $"{c.SubmissionDate:yyyy-MM-dd}," +
                    $"{c.HoursWorked}," +
                    $"{c.HourlyRate}," +
                    $"{total}," +
                    $"{c.Status}," +
                    $"{c.Comments?.Replace(",", " ")}";

                lines.Add(line);
            }

            var csv = string.Join("\n", lines);
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);

            return File(bytes, "text/csv", "claims_export.csv");
        }
    }
}
