using System.ComponentModel.DataAnnotations;

namespace ClaimSystem.Models
{
    public class RegisterViewModel
    {

        [Required]
        public string? Name { get; set; }


        public string? Surname { get; set; }


        public string? Department { get; set; }


        public decimal DefaultRatePerJob { get; set; }


        public string? RoleName { get; set; }


        public string? Email { get; set; }


        public string? TempPassword { get; set; }
    }
}