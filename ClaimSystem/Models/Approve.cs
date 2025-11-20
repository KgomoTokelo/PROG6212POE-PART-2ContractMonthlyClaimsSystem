using System.ComponentModel.DataAnnotations;

namespace ClaimSystem.Models
{
    public class Approve
    {
        [Key]
        public int ApprovalID { get; set; }   // PK
        public int ClaimID { get; set; }      // FK to claims
        public int UserID { get; set; }       // FK to User
        public DateTime ApprovalDate { get; set; }
        public string Decision { get; set; }  // Approved / Rejected
        public string Comments { get; set; }

        // Navigation
        public Claims Claim { get; set; }
        public Users? User { get; set; }

        
    }
}
