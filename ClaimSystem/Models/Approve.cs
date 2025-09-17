namespace ClaimSystem.Models
{
    public class Approve
    {
        public int ApprovalID { get; set; }   // PK
        public int ClaimID { get; set; }      // FK to claims
        public int UserID { get; set; }       // FK to User
        public DateTime ApprovalDate { get; set; }
        public string Decision { get; set; }  // Approved / Rejected
        public string Comments { get; set; }

        // Navigation
        public Claim Claim { get; set; }
        public User User { get; set; }
    }
}
