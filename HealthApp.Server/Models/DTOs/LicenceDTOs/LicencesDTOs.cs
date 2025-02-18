namespace HealthApp.Server.Models.DTOs.LicenceDTOs
{
    public class CreateLicenceDto
    {
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int PracticesId { get; set; }
    }

    // ✅ DTO for updating a licence
    public class UpdateLicenceDto
    {
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int PracticesId { get; set; }
    }

    // ✅ DTO for returning licence data in API responses
    public class LicenceDto
    {
        public int Id { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string PracticeName { get; set; } = null!;
    }
    public class AssignLicenceDto
    {
        public int DoctorId { get; set; }
        public int LicenceId { get; set; }
    }
}
