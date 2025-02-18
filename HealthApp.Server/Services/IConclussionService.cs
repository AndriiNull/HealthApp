using HealthApp.Server.Models.DTOs.ConclussionDTOs;

namespace HealthApp.Server.Services
{
    public interface IConclusionService
    {
        Task<List<ConclusionReadDto>> GetAllConclusionsAsync();
        Task<ConclusionReadDto?> GetConclusionByIdAsync(int id);
        Task<ConclusionReadDto> CreateConclusionAsync(ConclusionCreateDto dto);
        Task<bool> DeleteConclusionAsync(int id);
    }
}
