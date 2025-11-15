using System.ComponentModel.DataAnnotations;
using ABCRetailers.Models;

namespace ABCRetailers.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;



        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select your role.")]
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