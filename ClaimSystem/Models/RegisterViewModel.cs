using System.ComponentModel.DataAnnotations;

namespace ClaimSystem.Models
{
    public class RegisterViewModel
    {

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public string Department { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string RoleName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string TempPassword { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Rate must be 0 or higher")]
        public decimal DefaultRatePerJob { get; set; }
    }
}