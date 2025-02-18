using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HealthApp.Server.Models.DatabaseModels;

public class Analysis
{
    public int Id { get; set; }
    [ForeignKey(nameof(IssueNavigation))]
    [Column("Issue")]
    public int Issue { get; set; }
    public int Category { get; set; }
    public string Value { get; set; }
    [JsonIgnore]
    public virtual Issue? IssueNavigation { get; set; } // Navigation to Issue

    [JsonIgnore]
    public virtual Category? CategoryNavigation { get; set; } // Navigation to Category

    [JsonIgnore]
    public virtual ICollection<AnalysisConclusion>? AnalysisConclusions { get; set; } = new List<AnalysisConclusion>();
}
