using HealthApp.Server.Models;
using Microsoft.EntityFrameworkCore;
using HealthApp.Server.Models.DTOs.LicenceDTOs;
using HealthApp.Server.Models.DatabaseModels;

namespace HealthApp.Server.Services
{
    public class LicenceService
    {
        private readonly MasterContext _context;

        public LicenceService(MasterContext context)
        {
            _context = context;
        }

        // Get all licences
        public async Task<(bool Success, string Message, IEnumerable<LicenceDto>? Data)> GetLicencesAsync()
        {
            try
            {
                var licences = await _context.Licences
                    .Include(l => l.Practices)
                    .Select(l => new LicenceDto
                    {
                        Id = l.Id,
                        IssueDate = l.IssueDate,
                        ExpirationDate = l.ExpirationDate,
                        PracticeName = l.Practices.Practice
                    })
                    .ToListAsync();

                return (true, "Licences retrieved successfully.", licences);
            }
            catch (Exception ex)
            {
                return (false, $"Error retrieving licences: {ex.Message}", null);
            }
        }

        // Get a single licence by ID
        public async Task<(bool Success, string Message, LicenceDto? Data)> GetLicenceByIdAsync(int id)
        {
            try
            {
                var licence = await _context.Licences
                    .Include(l => l.Practices)
                    .Where(l => l.Id == id)
                    .Select(l => new LicenceDto
                    {
                        Id = l.Id,
                        IssueDate = l.IssueDate,
                        ExpirationDate = l.ExpirationDate,
                        PracticeName = l.Practices.Practice
                    })
                    .FirstOrDefaultAsync();

                if (licence == null)
                    return (false, $"Licence with ID {id} not found.", null);

                return (true, "Licence retrieved successfully.", licence);
            }
            catch (Exception ex)
            {
                return (false, $"Error retrieving licence: {ex.Message}", null);
            }
        }

        // Create a new licence
        public async Task<(bool Success, string Message, int? LicenceId)> CreateLicenceAsync(CreateLicenceDto licenceDto)
        {
            try
            {
                var newLicence = new Licence
                {
                    IssueDate = licenceDto.IssueDate,
                    ExpirationDate = licenceDto.ExpirationDate,
                    PracticesId = licenceDto.PracticesId
                };

                _context.Licences.Add(newLicence);
                await _context.SaveChangesAsync();

                return (true, "Licence created successfully.", newLicence.Id);
            }
            catch (Exception ex)
            {
                return (false, $"Error creating licence: {ex.Message}", null);
            }
        }

        // Assign a licence to a doctor
        public async Task<(bool Success, string Message)> AssignLicenceToDoctorAsync(AssignLicenceDto assignLicenceDto)
        {
            try
            {
                if (assignLicenceDto.DoctorId <= 0 || assignLicenceDto.LicenceId <= 0)
                    return (false, "Invalid DoctorId or LicenceId provided.");

                var doctor = await _context.Doctors.FindAsync(assignLicenceDto.DoctorId);
                if (doctor == null)
                    return (false, $"Doctor with ID {assignLicenceDto.DoctorId} not found.");

                var licence = await _context.Licences.FindAsync(assignLicenceDto.LicenceId);
                if (licence == null)
                    return (false, $"Licence with ID {assignLicenceDto.LicenceId} not found.");

                var existingAssignment = await _context.DoctorsLicences
                    .FirstOrDefaultAsync(dl => dl.DoctorId == assignLicenceDto.DoctorId && dl.LicenceId == assignLicenceDto.LicenceId);

                if (existingAssignment != null)
                    return (false, "This licence is already assigned to the doctor.");

                var newAssignment = new DoctorsLicences
                {
                    DoctorId = assignLicenceDto.DoctorId,
                    LicenceId = assignLicenceDto.LicenceId
                };

                _context.DoctorsLicences.Add(newAssignment);
                await _context.SaveChangesAsync();

                return (true, "Licence assigned successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error assigning licence: {ex.Message}");
            }
        }
    }
}
