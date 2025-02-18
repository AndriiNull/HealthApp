using System;
using System.Collections.Generic;

namespace HealthApp.Server.Models.DatabaseModels;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ReferenceValue { get; set; }
    public bool Global { get; set; }
}