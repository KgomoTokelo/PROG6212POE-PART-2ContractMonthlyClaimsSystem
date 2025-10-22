using System.ComponentModel.DataAnnotations;

namespace ClaimSystem.Models
{
    public class Claim
    {
        [Key]
        public int ClaimID { get; set; }       // PK
        public int LecturerID { get; set; }    // FK to Lecturer
        public string ModuleName { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int HoursWorked { get; set; }

        public decimal HourlyRate { get; set; } //added for poe
        public status Status { get; set; }     // Draft, Submitted, Approved, Rejected, Settled
        public string Comments { get; set; }

        // Navigation
        public Lecturer? Lecturer { get; set; }
        public ICollection<UploadDocuments>? Documents { get; set; }
        public ICollection<Approve>? Approvals { get; set; }

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
