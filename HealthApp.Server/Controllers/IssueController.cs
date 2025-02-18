using HealthApp.Server.DTOs;
using HealthApp.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class IssueController : ControllerBase
    {
        private readonly IssueService _issueService;

        public IssueController(IssueService issueService)
        {
            _issueService = issueService;
        }
        [HttpPost]
        public async Task<ActionResult<IssueReadDto>> CreateIssue([FromBody] IssueCreateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Description))
                return BadRequest("Invalid issue data");

            var newIssue = await _issueService.CreateIssueAsync(dto);
            return CreatedAtAction(nameof(GetIssuesByPatient), new { patientId = newIssue.PatientId }, newIssue);
            
        }
        [HttpGet("my-issues")]
        public async Task<IActionResult> GetUserIssues()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Extract `sub` claim (userId from JWT)

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID is missing in token.");
            }

            var issues = await _issueService.GetIssuesByUserId(userId); // Fetch issues by userId
            return Ok(issues);
        }
        [HttpGet("patient")]
        public async Task<ActionResult<IEnumerable<IssueReadDto>>> GetIssuesByPatient()
        {
            var requestorId = User.FindFirst("PersonId")?.Value; // ✅ Extract PersonId from JWT

            if (string.IsNullOrEmpty(requestorId))
                return Unauthorized("Invalid authentication token.");

            try
            {
                var issues = await _issueService.GetIssuesByUserId(requestorId);
                return Ok(issues);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("appointments/{issueId}")]
        public async Task<IActionResult> GetPreviousAppointments(int issueId)
        {
            var appointments = await _issueService.GetPreviousAppointments(issueId);
            return Ok(appointments);
        }

        // ✅ Get issue details by issueId
        [HttpGet("{issueId}")]
        public async Task<IActionResult> GetIssueDetails(int issueId)
        {
            var issue = await _issueService.GetIssueDetails(issueId);
            if (issue == null) return NotFound("Issue not found");

            return Ok(issue);
        }

        // ✅ Add a new recommendation to an issue
        [HttpPost("{issueId}/add")]
        public async Task<IActionResult> AddRecommendation(int issueId, [FromBody] AddRecommendationDto model)
        {
            var doctorId = GetDoctorIdFromToken(); // Replace this with actual identity logic
            bool success = await _issueService.AddRecommendation(issueId, model.Text, doctorId);

            if (!success) return BadRequest("Failed to add recommendation");

            return Ok(new { message = "Recommendation added successfully" });
        }
        private int GetDoctorIdFromToken()
        {
            // ✅ Replace with real logic to get doctorId from JWT
            return 1; // Dummy value for now
        }
        [HttpGet("analysis/{issueId}")]
        [Authorize(Roles = "Doctor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetIssueAnalysis(int issueId)
        {
            var analysisResults = await _issueService.GetIssueAnalysisAsync(issueId);

            if (analysisResults == null)
                return NotFound("Issue not found");

            return Ok(analysisResults);
        }
    }



}
    public class AddRecommendationDto
    {
        public string Text { get; set; }
    }

