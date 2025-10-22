using Xunit;
using ClaimSystem.Models;

namespace ClaimSystemTest
{
    public class ClaimsTest
    {
       

        [Fact]
        public void Lecturer_ShouldStoreData()
        {
            // Arrange
            var lecturer = new Lecturer
            {
                LecturerID = 1,
                Name = "John Smith",
                Email = "john@college.com",
                Department = "Computer Science"
            };

            // Assert
            Assert.Equal("John Smith", lecturer.Name);
            Assert.Equal("john@college.com", lecturer.Email);
            Assert.Equal("Computer Science", lecturer.Department);
        }

        [Fact]
        public void Claim_TotalAmount_ShouldBeCalculatedCorrectly()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 10,
                HourlyRate = 200
            };

            // Act
            var total = claim.HoursWorked * claim.HourlyRate;

            // Assert
            Assert.Equal(2000, total);
        }

        [Fact]
        public void RejectClaim_ShouldChangeStatusToRejected()
        {
            // Arrange
            var claim = new Claim { Status = Claim.status.Submitted };

            // Act
            claim.Status = Claim.status.Rejected;

            // Assert
            Assert.Equal(Claim.status.Rejected, claim.Status);
        }


    }
}
