using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HealthApp.Server.Models.DatabaseModels
{
   
        public class AppointmentComment
        {
            [Key]
            public int Id { get; set; }

            [ForeignKey("Appointment")]
            public int AppointmentId { get; set; }  // Links to the appointment

            public string CommentText { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
            [JsonIgnore]
            public virtual Appointment Appointment { get; set; }
        }
    }

