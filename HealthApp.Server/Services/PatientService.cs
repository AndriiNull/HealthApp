using HealthApp.Server.Models;
using HealthApp.Server.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using static HealthApp.Server.Models.DTOs.PatientDTOs.PatientDTOs;

namespace HealthApp.Server.Services
{
    public class PatientService
    {
        private readonly MasterContext _context;

        public PatientService(MasterContext context)
        {
            _context = context;
        }

        // ✅ Get All Patients
        public async Task<(bool Success, string Message, IEnumerable<PatientDto>? Data)> GetPatientsAsync()
        {
            try
            {
                var patients = await _context.Patients
                    .Include(p => p.PersonNavigation)
                    .Select(p => new PatientDto
                    {
                        Id = p.PersonId,
                        PersonId = p.PersonId,
                        Name = p.PersonNavigation.Name,
                        Surname = p.PersonNavigation.Surname,
                        Gender = p.PersonNavigation.Gender
                    })
                    .ToListAsync();

                return (true, "Patients retrieved successfully.", patients);
            }
            catch (Exception ex)
            {
                return (false, $"Error retrieving patients: {ex.Message}", null);
            }
        }

        // ✅ Get Patient by ID
        public async Task<(bool Success, string Message, PatientDto? Data)> GetPatientByIdAsync(int id)
        {
            try
            {
                var patient = await _context.Patients
                    .Include(p => p.PersonNavigation)
                    .Where(p => p.PersonId == id)
                    .Select(p => new PatientDto
                    {
                        Id = p.PersonId,
                        PersonId = p.PersonId,
                        Name = p.PersonNavigation.Name,
                        Surname = p.PersonNavigation.Surname,
                        Gender = p.PersonNavigation.Gender
                    })
                    .FirstOrDefaultAsync();

                if (patient == null)
                    return (false, $"Patient with ID {id} not found.", null);

                return (true, "Patient retrieved successfully.", patient);
            }
            catch (Exception ex)
            {
                return (false, $"Error retrieving patient: {ex.Message}", null);
            }
        }

        // ✅ Create Patient
        public async Task<(bool Success, string Message, int? PatientId)> CreatePatientAsync(CreatePatientDto patientDto)
        {
            try
            {
                Person? person = null;
                if (patientDto.PersonId != null)
                {
                    person = await _context.People.FirstOrDefaultAsync(e => e.Id == patientDto.PersonId);
                    if (person == null)
                        return (false, "Person id not found, please try another value, or if you dont have any - leave field empty", null);
                }

                // If no person was found OR PersonId was null, create a new person
                if (patientDto.PersonId == null  )
                {
                    person = new Person
                    {
                        Name = patientDto.Name,
                        Surname = patientDto.Surname,
                        Gender = patientDto.Gender
                    };

                    await _context.People.AddAsync(person);
                    await _context.SaveChangesAsync();
                }

                var newPatient = new Patient { PersonId = person.Id };
                await _context.Patients.AddAsync(newPatient);
                await _context.SaveChangesAsync();

                return (true, "Patient created successfully.", newPatient.PersonId);
            }
            catch (Exception ex)
            {
                return (false, $"Error creating patient: {ex.Message}", null);
            }
        }

        // ✅ Update Patient
        public async Task<(bool Success, string Message)> UpdatePatientAsync(int id, UpdatePatientDto updateDto)
        {
            try
            {
                var patient = await _context.Patients.Include(p => p.PersonNavigation).FirstOrDefaultAsync(p => p.PersonId == id);
                if (patient == null)
                    return (false, $"Patient with ID {id} not found.");

                if (!string.IsNullOrEmpty(updateDto.Name))
                    patient.PersonNavigation.Name = updateDto.Name;
                if (!string.IsNullOrEmpty(updateDto.Surname))
                    patient.PersonNavigation.Surname = updateDto.Surname;
                if (!string.IsNullOrEmpty(updateDto.Gender))
                    patient.PersonNavigation.Gender = updateDto.Gender;

                await _context.SaveChangesAsync();
                return (true, "Patient updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating patient: {ex.Message}");
            }
        }

        // ✅ Delete Patient
        public async Task<(bool Success, string Message)> DeletePatientAsync(int id)
        {
            try
            {
                var patient = await _context.Patients.Include(p => p.PersonNavigation).FirstOrDefaultAsync(p => p.PersonId == id);
                if (patient == null)
                    return (false, $"Patient with ID {id} not found.");

                _context.People.Remove(patient.PersonNavigation);
                await _context.SaveChangesAsync();

                return (true, "Patient deleted successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting patient: {ex.Message}");
            }
        }
    }
}
