using System.ComponentModel.DataAnnotations;

namespace ClaimSystem.Models
{
    public class Users
    {

            [Key]
            public int Id { get; set; }

            // Foreign Key reference to IdentityUser
            [Required]
            public string IdentityUserId { get; set; }

            // Profile data
            [Required]
            public string Name { get; set; }

            [Required]
            public string Surname { get; set; }

            [Required]
            public string Department { get; set; }

        [Required]
        [Range(0, 999999)]
        public decimal DefaultRatePerJob { get; set; }

        [Required]
        public string RoleName { get; set; }    

            public role Role { get; set; }

            public enum role
            {
                Lecturer,
                Coordinator,
                Manager,
                HumanResource
            }
        }
    }


    // Navigation
   
