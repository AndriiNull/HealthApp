namespace HealthApp.Server.Models.DTOs.ConclussionDTOs
{
    public class ConclusionReadDto
    {
        public int Id { get; set; }
        public DateTime IssueDate { get; set; }
        public List<AnalysisConclusionReadDto>? AnalysisConclusions { get; set; } = new List<AnalysisConclusionReadDto>();
    }

    public class ConclusionCreateDto
    {
        public DateTime IssueDate { get; set; }
        public List<AnalysisConclusionCreateDto> AnalysisConclusions { get; set; } = new List<AnalysisConclusionCreateDto>();
    }

    public class AnalysisConclusionReadDto
    {
        public int Id { get; set; }
        public int AnalysisId { get; set; }
        public int DoctorId { get; set; }
    }

    public class AnalysisConclusionCreateDto
    {
        public int AnalysisId { get; set; }
        public int DoctorId { get; set; }
    }
}
