using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using  HealthApp.Server.Models.DatabaseModels;

namespace HealthApp.Server.Models.DatabaseModels;

public class Appointment
{
    public int Id { get; set; }
    public int Issue { get; set; }
    public int Doctor { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Status { get; set; }
    public virtual Issue? IssueNavigation { get; set; }
    public virtual Doctor? DoctorNavigation { get; set; }
    // Recurring Appointment Reference
    [ForeignKey("PreviousAppointment")]
    public int? PreviousAppointmentId { get; set; }

    [JsonIgnore]
    public virtual Appointment? PreviousAppointment { get; set; }
    [JsonIgnore]
    public virtual ICollection<AppointmentComment>? Comments { get; set; } = new List<AppointmentComment>();
}


