using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;


[Authorize(Roles = "Manager", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/roles")]
public class RoleManagementController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleManagementController(IRoleService roleService)
    {
        _roleService = roleService;
    }
    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
    {
        if (!RoleConstants.ValidRoles.Contains(request.Role))
            throw new ArgumentException("Invalid role specified.");

        var result = await _roleService.AssignRoleAsync(request.PersonId, request.Role);
        if (!result)
            return BadRequest("Failed to assign role.");

        return Ok(new { Message = "Role assigned successfully." });
    }
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _roleService.GetUsersAsync();
        return Ok(users);
    }
    public class AssignRoleRequest
    {
        public int PersonId { get; set; }
        public string Role { get; set; }
    }
    public static class RoleConstants
    {
        public static readonly List<string> ValidRoles = new List<string>
    {
        "Admin",
        "Manager",
        "Doctor",
        "Nurse"
    };
    }
}
