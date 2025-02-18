using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HealthApp.Server.Models.DatabaseModels;

public class Diagnosis
{
    public int Id { get; set; }
    public int Issue { get; set; }
    public int Doctor { get; set; }
    [JsonIgnore]
    public virtual Issue? IssueNavigation { get; set; }
    [JsonIgnore]
    public virtual Doctor? DoctorNavigation { get; set; }
}