using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClaimSystem.Models
{
    public class Lecturer
    {
        [Key]
        public int LecturerID { get; set; }   // PK

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        [Range(0, 999999)]
        public decimal DefaultRatePerJob { get; set; }

        // Navigation
        public ICollection<Claims> Claims { get; set; }

        public int UsersId { get; set; }

        // Navigation with explicit FK name
        [ForeignKey(nameof(UsersId))]
        public Users Users { get; set; }

    }
}
