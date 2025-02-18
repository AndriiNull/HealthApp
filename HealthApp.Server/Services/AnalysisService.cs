using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthApp.Server.DTOs;
using HealthApp.Server.Models;
using HealthApp.Server.Models.DatabaseModels;

public class AnalysisService : IAnalysisService
{
    private readonly MasterContext _context;

    public AnalysisService(MasterContext context)
    {
        _context = context;
    }

    public async Task<List<AnalysisReadDto>> GetAllAnalysisAsync()
    {
        //return await _context.Analyses
        //    .Include(a => a.CategoryNavigation)
        //    .Select(a => new AnalysisReadDto
        //    {
        //        Id = a.Id,
        //        IssueId = a.Issue,
        //        CategoryId = a.Category,
        //        CategoryName = a.CategoryNavigation.Name,
        //        Value = a.Value
        //    }).ToListAsync();
        return null;
    }

    public async Task<AnalysisReadDto?> GetAnalysisByIdAsync(int id)
    {
        //var analysis = await _context.Analyses
        //    .Include(a => a.CategoryNavigation)
        //    .FirstOrDefaultAsync(a => a.Id == id);

        //if (analysis == null) return null;

        //return new AnalysisReadDto
        //{
        //    Id = analysis.Id,
        //    IssueId = analysis.Issue,
        //    CategoryId = analysis.Category,
        //    CategoryName = analysis.CategoryNavigation.Name,
        //    //Value = analysis.Value
        //};
        return null;
    }

    public async Task<AnalysisReadDto> CreateAnalysisAsync(AnalysisCreateDto analysisDto)
    {
        //var newAnalysis = new Analysis
        //{
        //    Issue = analysisDto.IssueId,
        //    Category = analysisDto.CategoryId,
        //    Value = analysisDto.Value
        //};

        //_context.Analyses.Add(newAnalysis);
        //await _context.SaveChangesAsync();

        //return new AnalysisReadDto
        //{
        //    Id = newAnalysis.Id,
        //    IssueId = newAnalysis.Issue,
        //    CategoryId = newAnalysis.Category,
        //    Value = newAnalysis.Value
        //};
        return null;
    }

    public async Task<bool> UpdateAnalysisAsync(int id, AnalysisUpdateDto analysisDto)
    {
        //var analysis = await _context.Analyses.FindAsync(id);
        //if (analysis == null) return false;

        //analysis.Value = analysisDto.Value;
        //await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAnalysisAsync(int id)
    {
        var analysis = await _context.Analyses.FindAsync(id);
        if (analysis == null) return false;

        _context.Analyses.Remove(analysis);
        await _context.SaveChangesAsync();
        return true;
    }
}
