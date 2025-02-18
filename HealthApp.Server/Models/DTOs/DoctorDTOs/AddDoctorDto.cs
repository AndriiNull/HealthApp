using HealthApp.Server.Models.DTOs.LicenceDTOs;

namespace HealthApp.Server.DTOs
{
    // ✅ DTO for Reading Doctors (GET)
    public class DoctorSummaryDto
    {
        public int DoctorId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public List<LicenceDto> ActiveLicenses { get; set; } 
        public DateTime? NextAvailableSlot { get; set; } // ✅ Rounded appointment time
    }

    public class DoctorReadDto
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime LicenseExpiration { get; set; }
    }

    // ✅ DTO for Creating a Doctor (POST)
    public class DoctorCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime LicenseExpiration { get; set; }
        public int? PersonId { get; set; }

    }

    // ✅ DTO for Updating a Doctor (PUT)
    public class DoctorUpdateDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Gender { get; set; }
        public DateTime? LicenseExpiration { get; set; }
    }

    // ✅ DTO for Deleting a Doctor (DELETE)
    public class DoctorDeleteDto
    {
        public int Id { get; set; }
    }
}
