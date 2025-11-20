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
    [Authorize(Roles = "HR")]
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


            public async Task<IActionResult> EmployeeList()
            {

                var employees = await _context.Users
                    .ToListAsync();


                return View(employees);
            }


            public IActionResult AddEmployee()
            {
                return View(new RegisterViewModel());
            }



            [HttpPost]
            [ValidateAntiForgeryToken]


            public async Task<IActionResult> AddEmployee(AddEmployeeViewModel model)
            {
                var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);

                if (!roleExists)
                {
                    ModelState.AddModelError("", $"Role '{model.RoleName}' does not exist.");
                }

                if (ModelState.IsValid)
                {
                    var user = new IdentityUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        EmailConfirmed = true
                    };

                    var createResult = await _userManager.CreateAsync(user, model.TempPassword);

                    if (createResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, model.RoleName);

                        var profile = new EmployeeProfile
                        {
                            UserId = user.Id,
                            Name = model.Name,
                            Surname = model.Surname,
                            Department = model.Department,
                            DefaultRatePerJob = model.DefaultRatePerJob,
                            RoleName = model.RoleName
                        };

                        _context.EmployeeProfiles.Add(profile);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(EmployeeList));
                    }

                    foreach (var error in createResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

                return View(model);
            }
            [Authorize(Roles = "HR")]
            public async Task<IActionResult> HrSummary()
            {
                var claims = await _context.WorkClaims.ToListAsync();

                var totalCount = claims.Count;
                var totalAmount = claims.Sum(c => c.TotalAmount);
                var submittedCount = claims.Count(c => c.Status == "Submitted");
                var pmRejectedCount = claims.Count(c => c.Status == "PM Rejected");
                var cmRejectedCount = claims.Count(c => c.Status == "CM Rejected");
                var approvedCount = claims.Count(c => c.Status == "CM Approved");
                var paidCount = claims.Count(c => c.Status == "Paid");

                ViewBag.TotalCount = totalCount;
                ViewBag.TotalAmount = totalAmount;
                ViewBag.SubmittedCount = submittedCount;
                ViewBag.PmRejectedCount = pmRejectedCount;
                ViewBag.CmRejectedCount = cmRejectedCount;
                ViewBag.ApprovedCount = approvedCount;
                ViewBag.PaidCount = paidCount;

                return View();
            }

            [Authorize(Roles = "HR")]
            public async Task<FileResult> HrExportCsv()
            {
                var claims = await _context.WorkClaims
                    .OrderBy(c => c.Id)
                    .ToListAsync();

                var lines = new List<string>();
                lines.Add("Id,Date,Name,Surname,Department,RatePerJob,NumberOfJobs,TotalAmount,Status,RejectReason");

                foreach (var c in claims)
                {
                    var line =
                        $"{c.Id}," +
                        $"{c.CreatedAt:yyyy-MM-dd}," +
                        $"{c.Name}," +
                        $"{c.Surname}," +
                        $"{c.Department}," +
                        $"{c.RatePerJob}," +
                        $"{c.NumberOfJobs}," +
                        $"{c.TotalAmount}," +
                        $"{c.Status}," +
                        $"{c.RejectReason?.Replace(",", " ")}";

                    lines.Add(line);
                }

                var csv = string.Join("\n", lines);
                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);

                return File(bytes, "text/csv", "claims_export.csv");
            }


        }
    }
