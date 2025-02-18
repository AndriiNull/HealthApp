using HealthApp.Server.Models.DatabaseModels;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

public class ApplicationRole : IdentityRole
{
    // ✅ Default Constructor (Required by EF Core)
    public ApplicationRole() : base() { }

    // ✅ Constructor to Set Role Name
    public ApplicationRole(string roleName, string description = "") : base(roleName)
    {
        Name = roleName; // Ensure Name is explicitly set
        Description = description;
    }

    // ✅ Optional Description (Good for UI/Admin Panel)
    public string Description { get; set; } = string.Empty;

    // ✅ Many-to-Many Relationship with Users
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new HashSet<ApplicationUserRole>();
}
