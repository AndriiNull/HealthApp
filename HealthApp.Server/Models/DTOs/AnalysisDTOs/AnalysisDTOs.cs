namespace HealthApp.Server.DTOs
{
    public class AnalysisReadDto
    {
        public int Id { get; set; }
        public int IssueId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public double Value { get; set; }
    }

    public class AnalysisCreateDto
    {
        public int IssueId { get; set; }
        public int CategoryId { get; set; }
        public double Value { get; set; }
    }

    public class AnalysisUpdateDto
    {
        public double Value { get; set; }
    }
}
