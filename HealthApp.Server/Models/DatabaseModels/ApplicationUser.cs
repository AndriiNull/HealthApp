using HealthApp.Server.Models.DatabaseModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


public class ApplicationUser : IdentityUser
{

    public ApplicationUser() : base() { }
    // ✅ Connects to Person (One-to-One)
    public int PersonId { get; set; }  // ✅ Foreign Key
    public virtual Person Person { get; set; } // 🔹 One-to-One

    // ✅ Many-to-Many: ApplicationUser can have multiple roles
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } 

}