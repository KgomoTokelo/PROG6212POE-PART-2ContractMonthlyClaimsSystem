using System.ComponentModel.DataAnnotations;

namespace ClaimSystem.Models
{
    public class UploadDocuments
    {
        [Key]
        public int DocumentID { get; set; }   // PK
        public int ClaimID { get; set; }      // FK to Claim
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; }

        // Navigation
        public Claim Claim { get; set; }
    }
}
