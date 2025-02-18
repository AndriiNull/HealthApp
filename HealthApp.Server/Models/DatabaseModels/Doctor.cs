using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HealthApp.Server.Models.DatabaseModels;

public class Doctor
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Person")]
    public int PersonId { get; set; }  // Foreign Key to Person

    public virtual Person Person { get; set; }  // Navigation Property
    [JsonIgnore]
    public virtual ICollection<DoctorsLicences>? DoctorsLicences { get; set; }
}
