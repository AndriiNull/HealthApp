using System;
using System.Collections.Generic;

namespace HealthApp.Server.Models.DatabaseModels;

public class Prescription
{
    public int Id { get; set; }
    public int Diagnosis { get; set; }
    public string? Dose { get; set; }
    public string Medicine { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool Overturned { get; set; }
    public virtual Diagnosis? DiagnosisNavigation { get; set; }
}