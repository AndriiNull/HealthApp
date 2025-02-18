using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthApp.Server.DTOs;
using HealthApp.Server.Models;
using HealthApp.Server.Models.DatabaseModels;
using HealthApp.Server.Models.DTOs.ConclussionDTOs;
using HealthApp.Server.Services;

public class ConclusionService : IConclusionService
{
    private readonly MasterContext _context;

    public ConclusionService(MasterContext context)
    {
        _context = context;
    }

    public async Task<List<ConclusionReadDto>> GetAllConclusionsAsync()
    {
        return await _context.Conclusions
            .Include(c => c.AnalysisConclusions) // Load AnalysisConclusion
            .ThenInclude(ac => ac.AnalysisNavigation) // Load Analysis details
            .Select(c => new ConclusionReadDto
            {
                Id = c.Id,
                IssueDate = c.IssueDate,
                AnalysisConclusions = c.AnalysisConclusions.Select(ac => new AnalysisConclusionReadDto
                {
                    Id = ac.Id,
                    AnalysisId = ac.Analysis,
                    DoctorId = ac.Doctor
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<ConclusionReadDto?> GetConclusionByIdAsync(int id)
    {
        var conclusion = await _context.Conclusions
            .Include(c => c.AnalysisConclusions)
            .ThenInclude(ac => ac.AnalysisNavigation)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (conclusion == null) return null;

        return new ConclusionReadDto
        {
            Id = conclusion.Id,
            IssueDate = conclusion.IssueDate,
            AnalysisConclusions = conclusion.AnalysisConclusions.Select(ac => new AnalysisConclusionReadDto
            {
                Id = ac.Id,
                AnalysisId = ac.Analysis,
                DoctorId = ac.Doctor
            }).ToList()
        };
    }

    public async Task<ConclusionReadDto> CreateConclusionAsync(ConclusionCreateDto dto)
    {
        var newConclusion = new Conclusion
        {
            IssueDate = dto.IssueDate
        };

        _context.Conclusions.Add(newConclusion);
        await _context.SaveChangesAsync();

        foreach (var acDto in dto.AnalysisConclusions)
        {
            var newAnalysisConclusion = new AnalysisConclusion
            {
                Analysis = acDto.AnalysisId,
                Conclusion = newConclusion.Id, // Link to the new conclusion
                Doctor = acDto.DoctorId
            };
            _context.AnalysisConclusions.Add(newAnalysisConclusion);
        }

        await _context.SaveChangesAsync();

        return new ConclusionReadDto
        {
            Id = newConclusion.Id,
            IssueDate = newConclusion.IssueDate,
            //AnalysisConclusions = dto.AnalysisConclusions
        };
    }

    public async Task<bool> DeleteConclusionAsync(int id)
    {
        var conclusion = await _context.Conclusions
            .Include(c => c.AnalysisConclusions)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (conclusion == null) return false;

        _context.AnalysisConclusions.RemoveRange(conclusion.AnalysisConclusions);
        _context.Conclusions.Remove(conclusion);
        await _context.SaveChangesAsync();
        return true;
    }
}
