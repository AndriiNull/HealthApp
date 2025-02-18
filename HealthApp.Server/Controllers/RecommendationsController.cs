using HealthApp.Server.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/recommendations")]
[ApiController]
public class RecommendationsController : ControllerBase
{
    private readonly MasterContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public RecommendationsController(MasterContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ✅ Get all recommendations for an issue
    [HttpGet("{issueId}")]
    public async Task<IActionResult> GetRecommendations(int issueId)
    {
        var recommendations = await _context.Recommendations
            .Where(r => r.IssueId == issueId)
            .Include(r => r.Doctor)
            .Select(r => new
            {
                r.Id,
                r.Text,
                DoctorName = r.Doctor.Person.Name,
                r.CreatedAt
            })
            .ToListAsync();

        return Ok(recommendations);
    }

    // ✅ Add a new recommendation (Only Doctors)
    [HttpPost("{issueId}/add")]
    [Authorize(Roles = "Doctor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> AddRecommendation(int issueId, [FromBody] string recommendationText)
    {
        if (string.IsNullOrWhiteSpace(recommendationText))
            return BadRequest("Recommendation text cannot be empty.");
        var person = await _userManager.GetUserAsync(User);
        var doctor = await _context.Doctors.FirstOrDefaultAsync(e=> e.PersonId == person.PersonId);
        if (doctor == null)
            return Unauthorized("Invalid authentication token.");
        Console.WriteLine($"📥 Received Recommendation: {recommendationText} for Issue {issueId}");

        var doctorId = doctor.Id; // ✅ Extract DoctorId from JWT


        var issue = await _context.Issues.FindAsync(issueId);
        if (issue == null) return NotFound("Issue not found");

        var recommendation = new Recommendation
        {
            Text = recommendationText.Trim(), // ✅ Ensure trimmed text
            IssueId = issue.Id,
            DoctorId = doctorId,
            CreatedAt = DateTime.UtcNow // ✅ Auto-generate timestamp
        };

        _context.Recommendations.Add(recommendation);
        await _context.SaveChangesAsync();

        Console.WriteLine($"✅ Recommendation successfully saved: {recommendation.Text}");

        return Ok(new { message = "Recommendation added successfully" });
    }

}
