using System.Collections.Generic;
using System.Threading.Tasks;
using HealthApp.Server.DTOs;

public interface IAnalysisService
{
    Task<List<AnalysisReadDto>> GetAllAnalysisAsync();
    Task<AnalysisReadDto?> GetAnalysisByIdAsync(int id);
    Task<AnalysisReadDto> CreateAnalysisAsync(AnalysisCreateDto analysisDto);
    Task<bool> UpdateAnalysisAsync(int id, AnalysisUpdateDto analysisDto);
    Task<bool> DeleteAnalysisAsync(int id);
}