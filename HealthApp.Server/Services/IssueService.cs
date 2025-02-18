using HealthApp.Server.DTOs;
using HealthApp.Server.Models;
using HealthApp.Server.Models.DatabaseModels;
using HealthApp.Server.Models.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static AnalysisController;

namespace HealthApp.Server.Services
{
    public class IssueService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MasterContext _context;
        public IssueService(UserManager<ApplicationUser> userManager, MasterContext context)
        {
            _userManager = userManager; // ✅ Identity UserManager
            _context = context; // ✅ Main DB Context
        }
        public async Task<IssueReadDto> CreateIssueAsync(IssueCreateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Description))
                throw new ArgumentException("Invalid issue data");

            var newIssue = new Issue
            {
                Patient = Int32.Parse(dto.PatientId),
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                Status = "Open"
            };

            _context.Issues.Add(newIssue);
            await _context.SaveChangesAsync();

            return new IssueReadDto
            {
                Id = newIssue.Id,
                PatientId = newIssue.Patient.ToString(),
                Description = newIssue.Description,
                CreatedAt = newIssue.CreatedAt
            };
        }
        public async Task<List<AppointmentSummaryDto>> GetPreviousAppointments(int issueId)
        {
            return await _context.Appointments
                .Where(a => a.Issue == issueId)
                .OrderByDescending(a => a.StartTime)
                .Select(a => new AppointmentSummaryDto
                {
                    Id = a.Id,
                    AppointmentDate = a.StartTime,
                    Status = a.Status,
                    DoctorName = a.DoctorNavigation.Person.Name,
                   // Recommendations = a.IssueNavigation.Recommendations
                        //.Select(r => new RecommendationDto
                        //{
                        //    Text = r.Text,
                        //    CreatedAt = r.CreatedAt,
                        //    DoctorName = r.Doctor.Person.Name
                        //}).ToList()
                })
                .ToListAsync();
        }
        public async Task<IssueDto?> GetIssueDetails(int issueId)
        {
            var x = await _context.Issues
                .Where(i => i.Id == issueId)
                .Select(i => new IssueDto
                {
                    Id = i.Id,
                    Title = i.Description,
                    Status = i.Status,
                    CreatedAt = i.CreatedAt,
                    Recommendations = i.Recommendations
                        .Select(r => new RecommendationDto
                        {
                            Text = r.Text,
                            CreatedAt = r.CreatedAt,
                            DoctorName = r.Doctor.Person.Name
                        }).ToList()
                })
                .FirstOrDefaultAsync();
            return x;
        }
        public async Task<List<Issue>> GetIssuesByUserId(string userId)
        {
            var person = await _userManager.Users.FirstOrDefaultAsync(e => e.Id == userId);
            return await _context.Issues
                .Where(i => i.Patient == person.PersonId) // Ensure `PatientId` is stored correctly as a string
                .ToListAsync();
        }
        public async Task<bool> AddRecommendation(int issueId, string text, int doctorId)
        {
            var issue = await _context.Issues.FindAsync(issueId);
            if (issue == null) return false;

            var recommendation = new Recommendation
            {
                IssueId = issueId,
                Text = text,
                DoctorId = doctorId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Recommendations.Add(recommendation);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<List<AnalysisDto>> GetIssueAnalysisAsync(int issueId)
        {
            var issue = await _context.Issues
                .FirstOrDefaultAsync(i => i.Id == issueId);

            if (issue == null)
                return null;

            return await _context.Analyses
                .Where(a => a.Issue == issueId)
                .Include(a => a.CategoryNavigation) // ✅ Ensure category details are fetched
                .Select(a => new AnalysisDto
                {
                    Id = a.Id,
                    Category = new CategoryDto
                    {
                        Name = a.CategoryNavigation.Name
                    },
                     Value = a.Value
                })
                .ToListAsync();
        }

    }
    public class AnalysisDto
    {
        public int Id { get; set; }
        public CategoryDto Category { get; set; }
        public string Value { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AppointmentSummaryDto
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; }
        public string DoctorName { get; set; }
       // public List<RecommendationDto> Recommendations { get; set; } = new();
    }
    public class IssueDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<RecommendationDto> Recommendations { get; set; } = new();
    }

    public class RecommendationDto
    {
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DoctorName { get; set; }
    }
}
