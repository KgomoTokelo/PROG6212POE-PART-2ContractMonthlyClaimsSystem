using System;
using System.Threading.Tasks;
using ClaimSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace ClaimSystem.Models
{

    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            // Get required services from the DI container.
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // 
            context.Database.Migrate();

            // 
            string[] roles = { "HumanResource", "Lecturer", "Coordinator", "Manager" };


            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 
            var hrEmail = "hr@site.com";
            var hrUser = await userManager.FindByEmailAsync(hrEmail);
            if (hrUser == null)
            {
                hrUser = new IdentityUser
                {
                    UserName = hrEmail,
                    Email = hrEmail,
                    EmailConfirmed = true
                };

                // 
                await userManager.CreateAsync(hrUser, "Hr@123!");

                await userManager.AddToRoleAsync(hrUser, "HumanResource");

                context.Users.Add(new Users
                {
                    IdentityUserId = hrUser.Id,
                    Name = "HR",
                    Surname = "Manager",
                    Department = "Admin",
                    DefaultRatePerJob = 0,
                    RoleName = "HumanResource",
                    Email = hrEmail,
                    Password = "Hr@123!"


                });

                await context.SaveChangesAsync();
            }

            // 
            var codEmail = "cod@site.com";
            var codUser = await userManager.FindByEmailAsync(codEmail);
            if (codUser == null)
            {
                codUser = new IdentityUser
                {
                    UserName = codEmail,
                    Email = codEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(codUser, "Cod@123!");
                await userManager.AddToRoleAsync(codUser, "Coordinator");

                context.Users.Add(new Users
                {
                    IdentityUserId = codUser.Id,
                    Name = "Project",
                    Surname = "Manager",
                    Department = "Projects",
                    DefaultRatePerJob = 0,
                    RoleName = "Coordinator",
                    Email = codEmail,
                    Password = "Cod@123!"
                });

                await context.SaveChangesAsync();
            }

            // 
            var manEmail = "pm@site.com";
            var manUser = await userManager.FindByEmailAsync(manEmail);
            if (manUser == null)
            {
                manUser = new IdentityUser
                {
                    UserName = manEmail,
                    Email = manEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(manUser, "Pm@123!");
                await userManager.AddToRoleAsync(manUser, "Manager");

                context.Users.Add(new Users
                {
                    IdentityUserId = manUser.Id,
                    Name = "Construction",
                    Surname = "Manager",
                    Department = "Construction",
                    DefaultRatePerJob = 0,
                    RoleName = "Manager",
                    Email = manEmail,
                    Password = "Pm@123!"
                });

                await context.SaveChangesAsync();
            }


            var LecturerEmail = "Lec@site.com";
            var LecturerUser = await userManager.FindByEmailAsync(LecturerEmail);
            if (LecturerUser == null)
            {
                LecturerUser = new IdentityUser
                {
                    UserName = LecturerEmail,
                    Email = LecturerEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(LecturerUser, "Lec@123!");
                await userManager.AddToRoleAsync(LecturerUser, "Lecturer");

                var userRecord = new Users
                {
                    IdentityUserId = LecturerUser.Id,
                    Name = "John",
                    Surname = "Builder",
                    Department = "Masonry",
                    DefaultRatePerJob = 200,
                    RoleName = "Lecturer",
                    Email = LecturerEmail,
                    Password = "Lec@123!"
                };

                context.Users.Add(userRecord);
                await context.SaveChangesAsync();

                // Now add the Lecturer record linked to the Users table
                var lecturerRecord = new Lecturer
                {
                    Name = userRecord.Name,
                    Surname = userRecord.Surname,
                    Department = userRecord.Department,
                    DefaultRatePerJob = userRecord.DefaultRatePerJob,
                    Email = userRecord.Email,
                    UsersId = userRecord.Id // link to Users table
                };



            }
        }
    }
}


