using System.ComponentModel.DataAnnotations;

namespace ClaimSystem.Models
{
    public class Claims
    {
        

        [Key]
        public int ClaimID { get; set; }       // PK
        public int LecturerID { get; set; }    // FK to Lecturer
        public string ModuleName { get; set; }
        public DateTime SubmissionDate { get; set; }

        [Range(2, 16, ErrorMessage = "Hours worked must be between 2 and 16 hours.")]
        public int HoursWorked { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(60.00, 98.00, ErrorMessage = "Amount must be between 60.00 and 98.00. Maximum Rates for Stem department is 98.00 and NON-Stem department 88.00 .")]
        public decimal HourlyRate { get; set; } //added for poe
        public status Status { get; set; }     // Draft, Submitted, Approved, Rejected, Settled
        public string Comments { get; set; }

        // Navigation
        public Lecturer? Lecturer { get; set; }
        public ICollection<UploadDocuments>? Documents { get; set; }
        public Approve? Approvals { get; set; }

        //created my own datatype because i have fixed values for status
        public enum status{
            Submitted,
            Verefied,
            Approved,
            Rejected,
            Decline,
            Settled
            }

    }
}
