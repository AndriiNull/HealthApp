using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HealthApp.Server.Models.DatabaseModels
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
       // public ApplicationUserRole() : base() { }
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }
    }
}
