using System.ComponentModel.DataAnnotations;

namespace ClaimSystem.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }     // PK
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public role Role { get; set; }    // Lecturer / Coordinator / Manager

        public enum role
        {
            Lecturer,
            Coordinator , 
            Manager,
            HumanResource
        }

        // Navigation
        public Approve Approvals { get; set; }
    }
}
