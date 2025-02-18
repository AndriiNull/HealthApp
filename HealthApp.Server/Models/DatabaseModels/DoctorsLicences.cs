using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HealthApp.Server.Models.DatabaseModels
{
    public class DoctorsLicences
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }

        public virtual Doctor Doctor { get; set; }

        [ForeignKey("Licence")]
        public int LicenceId { get; set; }
        [JsonIgnore]

        public virtual Licence Licence { get; set; }
    }
}
