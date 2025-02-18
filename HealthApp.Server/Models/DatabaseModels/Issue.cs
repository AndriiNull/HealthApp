using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HealthApp.Server.Models.DatabaseModels;

public class Issue
{
    public int Id { get; set; }
    public int Patient { get; set; }
    public int? LeadDoctor { get; set; }
    public string Description { get; set; } = "No description provided";
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; }
    [JsonIgnore]
    public virtual Patient? PatientNavigation { get; set; }
    [JsonIgnore]
    public virtual Doctor? LeadDoctorNavigation { get; set; }
    [JsonIgnore]
    public virtual ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();
    [JsonIgnore]
    public virtual ICollection<Analysis> Analyses { get; set; } = new List<Analysis>();
}

