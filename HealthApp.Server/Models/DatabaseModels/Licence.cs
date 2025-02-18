using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HealthApp.Server.Models.DatabaseModels
{
    public class Licence
    {
        [Key]
        public int Id { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        // ✅ Foreign Key to Practices
        public int PracticesId { get; set; }

        [JsonIgnore]
        public virtual Practices Practices { get; set; }

        // ✅ Many-to-Many Navigation to Doctors
        [JsonIgnore]
        public virtual ICollection<DoctorsLicences> DoctorsLicences { get; set; } = new List<DoctorsLicences>();
    }
}

