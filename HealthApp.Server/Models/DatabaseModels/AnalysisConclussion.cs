using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HealthApp.Server.Models.DatabaseModels;

public class AnalysisConclusion
{
    public int Id { get; set; }
    public int Analysis { get; set; }
    public int Conclusion { get; set; }
    public int Doctor { get; set; }
    [JsonIgnore]
    public virtual Analysis? AnalysisNavigation { get; set; }
    [JsonIgnore]
    public virtual Conclusion? ConclusionNavigation { get; set; }
    [JsonIgnore]
    public virtual Doctor? DoctorNavigation { get; set; }
}

