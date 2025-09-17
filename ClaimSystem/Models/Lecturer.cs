namespace ClaimSystem.Models
{
    public class Lecturer
    {
        public int LecturerID { get; set; }   // PK
        public string Name { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }

        // Navigation
        public ICollection<Claim> Claims { get; set; }
    }
}
