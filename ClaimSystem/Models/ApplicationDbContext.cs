using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ClaimSystem.Models
{
    public class ApplicationDbContext: IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options) 
        { }

        public DbSet<Claims> Claims { get; set; }
        public DbSet<Approve> Approves { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<UploadDocuments> UploadDocuments { get; set; }

        public DbSet<Users> Users { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fix decimal types to prevent truncation
            modelBuilder.Entity<Claims>()
                .Property(c => c.HourlyRate)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Lecturer>()
                .Property(l => l.DefaultRatePerJob)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Users>()
                .Property(u => u.DefaultRatePerJob)
                .HasColumnType("decimal(18,2)");

            // Fix cascade issue: Lecturer → Users
            modelBuilder.Entity<Lecturer>()
                .HasOne(l => l.Users)
                .WithMany()           // no navigation on Users side
                .HasForeignKey(l => l.UsersId)
                .OnDelete(DeleteBehavior.Restrict);  // prevents multiple cascade paths

            // Optional: Claims → Lecturer cascade stays
            modelBuilder.Entity<Claims>()
                .HasOne(c => c.Lecturer)
                .WithMany(l => l.Claims)
                .HasForeignKey(c => c.LecturerID)
                .OnDelete(DeleteBehavior.Cascade);   // safe, no cycle

            // Seed data if needed
        }







    }
}
