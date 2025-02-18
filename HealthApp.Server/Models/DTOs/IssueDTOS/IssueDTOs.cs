namespace HealthApp.Server.DTOs
{
    public class IssueCreateDto
    {
        public string PatientId { get; set; }
        public string Description { get; set; }
    }
    public class IssueReadDto
    {
        public int Id { get; set; }
        public string PatientId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}


