using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HealthApp.Server.Models.DatabaseModels;

public class Patient
{
    [Key, ForeignKey("Person")]
    public int PersonId { get; set; } //  Primary + Foreign Key
    [JsonIgnore]
    public virtual Person? PersonNavigation { get; set; }
}
