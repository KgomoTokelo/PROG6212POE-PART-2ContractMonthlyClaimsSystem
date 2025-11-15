using ClaimSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ABCRetailers.Data
{
    public class AuthDbContext : DbContext
    {

        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();

        public DbSet<Lecturer> Cart => Set<Lecturer>();
        public DbSet<Claim> Claim => Set<Claim>();
    }

}