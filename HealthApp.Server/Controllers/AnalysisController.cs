using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthApp.Server.Services;
using HealthApp.Server.DTOs;
using Microsoft.EntityFrameworkCore;
using HealthApp.Server.Models;
using HealthApp.Server.Models.DatabaseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

[Route("api/[controller]")]
[ApiController]
public class AnalysisController : ControllerBase
{
    private readonly IAnalysisService _analysisService;
    private readonly MasterContext _context;
   private readonly UserManager<ApplicationUser> _userManager;
    public AnalysisController(IAnalysisService analysisService, MasterContext context, UserManager<ApplicationUser> userManager)
    {
        _analysisService = analysisService;
        _context = context;
        _userManager = userManager;
    }

    // GET: api/analysis
    [HttpGet("{issueId}/analysis")]
    public async Task<IActionResult> GetAnalysisByIssue(int issueId)
    {
        var analysisData = await _context.Analyses.Where(a => a.Issue == issueId).ToListAsync();


        if (!analysisData.Any())
            return NotFound("No analysis found for this issue.");

        return Ok(analysisData);
    }
    [HttpPost("{issueId}/add")]
    [Authorize(Roles = "Doctor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> AddAnalysis(int issueId, [FromBody] AnalysisDto model)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(e=>e.Id == model.CategoryId);
        if (category == null)
            return BadRequest();
        var user = await _userManager.GetUserAsync(User);
        var personId = user.PersonId;
        var doctor = await _context.Doctors.FirstOrDefaultAsync(e => e.PersonId == personId);
        var doctorId = doctor.Id;
        if (doctorId == null)
            return Unauthorized("Invalid authentication token.");

        // ✅ Ensure the issue exists
        var issue = await _context.Issues.FindAsync(issueId);
        if (issue == null) return NotFound("Issue not found");

        // ✅ Find or create the category
        

        if (category == null)
        {
           return NotFound("Category not found");
        }

        // ✅ Create new analysis entry
        var analysis = new Analysis
        {
            Issue = issue.Id,
            Category = category.Id,
            Value = model.Value
        };

        _context.Analyses.Add(analysis);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Analysis added successfully" });
    }
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<object>>> GetCategories()
    {
        try
        {
            var categories = await _context.Categories
                .Select(c => new
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            if (!categories.Any())
                return NotFound("No categories found.");

            return Ok(categories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }   

    [HttpPost("categories")]
    [Authorize(Roles = "Doctor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> AddCategory([FromBody] CategoryDto categoryDto)
    {

        if (string.IsNullOrWhiteSpace(categoryDto.Name) || string.IsNullOrWhiteSpace(categoryDto.ReferenceValue))
            return BadRequest("Invalid category data.");

        var existingCategory = await _context.Categories.AnyAsync(c => c.Name == categoryDto.Name);
        if (existingCategory)
            return Conflict("Category already exists.");

        var category = new Category
        {
            Name = categoryDto.Name,
            ReferenceValue = categoryDto.ReferenceValue // ✅ Storing string reference
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return Ok(new { id = category.Id, name = category.Name, referenceValue = category.ReferenceValue });
    }

    public class CategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string ReferenceValue { get; set; } = string.Empty; // ✅ Now a string
    }


    public class AnalysisDto
    {
        public int CategoryId { get; set; } // ✅ ID of the selected category
        public string Value { get; set; } = string.Empty; // ✅ The analysis result value
    }

}
