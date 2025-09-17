namespace ClaimSystem.Models
{
    public class Claim
    {
        public int ClaimID { get; set; }       // PK
        public int LecturerID { get; set; }    // FK → Lecturer
        public string ModuleName { get; set; }
        public DateTime SubmissionDate { get; set; }
        public decimal HoursWorked { get; set; }
        public string Status { get; set; }     // Draft, Submitted, Approved, Rejected, Settled
        public string Comments { get; set; }

        // Navigation
        public Lecturer Lecturer { get; set; }
        public ICollection<UploadDocuments> Documents { get; set; }
        public ICollection<Approve> Approvals { get; set; }
    }
}
