using HealthApp.Server.Models.DatabaseModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Recommendation
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Text { get; set; }

    [Required]
    public int IssueId { get; set; } // 🔗 Foreign Key to Issues Table
    public virtual Issue Issue { get; set; } // 🔄 Navigation Property

    [Required]
    public int DoctorId { get; set; } // 🔗 Foreign Key to User (Doctor)
    public virtual Doctor Doctor { get; set; } // 🔄 Navigation Property

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
