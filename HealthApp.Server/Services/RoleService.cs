using HealthApp.Server.Models;
using HealthApp.Server.Models.DatabaseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class RoleService : IRoleService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly MasterContext _context;
    public RoleService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, MasterContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<bool> AssignRoleAsync(int perosnId, string role)
    {
        var person = await _context.People.FindAsync(perosnId);
        if (person == null)
        {
            return false;
        }
        if (role == "Doctor")
        {
            if (await _context.Doctors.AnyAsync(d => d.PersonId == perosnId))
            {
                var doctor = new Doctor { PersonId = perosnId };
                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();
            }
        }
        else if (role == "Patient")
        {
            if(await _context.Patients.AnyAsync(d => d.PersonId == perosnId))
            {
                var patient = new Patient { PersonId = perosnId };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }
        }
        
        var user = await _userManager.Users.Where(u => u.PersonId == perosnId).FirstOrDefaultAsync();
        if (user == null) return false;

        var result = await _userManager.AddToRoleAsync(user, role);
        return result.Succeeded;
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        // Step 1: Fetch all users
        var users = await _userManager.Users.ToListAsync();

        // Step 2: Fetch all roles and their users
        var roles = await _roleManager.Roles.ToListAsync(); // Get all roles

        var rolesByUser = new Dictionary<string, List<string>>();

        foreach (var role in roles)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name); // Get users per role
            foreach (var user in usersInRole)
            {
                if (!rolesByUser.ContainsKey(user.Id))
                    rolesByUser[user.Id] = new List<string>();

                rolesByUser[user.Id].Add(role.Name);
            }
        }

        // Step 3: Merge users with roles
        var userList = users.Select(user =>
        {
            var roles = rolesByUser.ContainsKey(user.Id) ? rolesByUser[user.Id] : new List<string>();
            return new UserDto
            {
                Id = user.PersonId.ToString(),
                Email = user.Email,
                Role = roles.Any() ? string.Join(", ", roles) : "No Role"
            };
        }).ToList();

        return userList;
    }



}
public class UserDto
{
    public string Id { get; set; }       // User's unique identifier
    public string Email { get; set; }    // User's email for display
    public string FullName { get; set; } // Optional: User's full name (if available)
    public string Role { get; set; }     // Current role of the user (e.g., "Doctor", "Manager")
}
