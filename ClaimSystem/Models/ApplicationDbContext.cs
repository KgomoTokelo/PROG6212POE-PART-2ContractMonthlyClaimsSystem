using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClaimSystem.Models
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options) 
        { }

        public DbSet<Claim> Claims { get; set; }
        public DbSet<Approve> Approves { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<UploadDocuments> UploadDocuments { get; set; }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Claim>()
                .Property(c => c.HourlyRate)
                .HasColumnType("decimal(18,2)"); // precision: 18, scale: 2

            

            // Seed lecturers
            
            
        }

        public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
        {
            public ApplicationDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlite("Data Source=app.db"); // SQLite database

                return new ApplicationDbContext(optionsBuilder.Options);
            }
        }




    }
}
