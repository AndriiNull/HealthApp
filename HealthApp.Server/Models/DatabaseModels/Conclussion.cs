using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HealthApp.Server.Models.DatabaseModels;

public class Conclusion
{
    [Key]
    public int Id { get; set; }
    public DateTime IssueDate { get; set; }

    [JsonIgnore]
    public virtual ICollection<AnalysisConclusion> AnalysisConclusions { get; set; } = new List<AnalysisConclusion>();
}