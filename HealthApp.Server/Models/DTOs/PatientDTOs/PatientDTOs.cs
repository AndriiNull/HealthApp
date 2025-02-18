namespace HealthApp.Server.Models.DTOs.PatientDTOs
{
    public class PatientDTOs
    {
        public class PatientDto
        {
            public int Id { get; set; }
            public int PersonId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;
            public string Gender { get; set; } = string.Empty;
        }
        public class CreatePatientDto
        {
            public string Name { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;
            public string Gender { get; set; } = string.Empty;
            public int? PersonId { get; set; }
        }
        public class UpdatePatientDto
        {
            public string? Name { get; set; }
            public string? Surname { get; set; }
            public string? Gender { get; set; }
        }
    }
}
