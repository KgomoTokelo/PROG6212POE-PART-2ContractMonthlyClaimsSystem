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

            modelBuilder.Entity<Claims>()
                .Property(c => c.HourlyRate)
                .HasColumnType("decimal(18,2)"); // precision: 18, scale: 2

            

            // Seed lecturers
            
            
        }

       




    }
}
