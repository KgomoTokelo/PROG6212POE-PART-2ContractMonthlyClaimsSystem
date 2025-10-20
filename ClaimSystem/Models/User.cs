using System.ComponentModel.DataAnnotations;

namespace ClaimSystem.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }     // PK
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }    // Lecturer / Coordinator / Manager

        // Navigation
        public ICollection<Approve> Approvals { get; set; }
    }
}
