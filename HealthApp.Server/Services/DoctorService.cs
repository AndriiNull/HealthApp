using HealthApp.Server.DTOs;
using HealthApp.Server.Models;
using HealthApp.Server.Models.DatabaseModels;
using HealthApp.Server.Models.DTOs;
using HealthApp.Server.Models.DTOs.LicenceDTOs;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Server.Services
{
    public class DoctorService
    {
        private readonly MasterContext _context;

        public DoctorService(MasterContext context)
        {
            _context = context;
        }

        // ✅ GET: Retrieve all doctors
        public async Task<IEnumerable<DoctorReadDto>> GetAllDoctorsAsync()
        {
            try
            {
                return await _context.Doctors
                    .Include(d => d.Person)
                    .Select(d => new DoctorReadDto
                    {
                        Id = d.Id,
                        PersonId = d.PersonId,
                        Name = d.Person.Name,
                        Surname = d.Person.Surname,
                        Gender = d.Person.Gender
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving doctors", ex);
            }
        }

        // ✅ GET: Retrieve a single doctor by ID
        public async Task<DoctorReadDto?> GetDoctorByIdAsync(int id)
        {
            try
            {
                var doctor = await _context.Doctors
                    .Include(d => d.Person)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (doctor == null) return null;

                return new DoctorReadDto
                {
                    Id = doctor.Id,
                    PersonId = doctor.PersonId,
                    Name = doctor.Person.Name,
                    Surname = doctor.Person.Surname,
                    Gender = doctor.Person.Gender
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving doctor with ID {id}", ex);
            }
        }

        // ✅ POST: Create a new doctor
        public async Task<DoctorReadDto?> CreateDoctorAsync(DoctorCreateDto doctorDto)
        {
            try
            {
                // Check if the PersonId is provided and find existing Person
                Person person;
                if (doctorDto.PersonId.HasValue)
                {
                    person = await _context.People.FirstOrDefaultAsync(p => p.Id == doctorDto.PersonId);

                    // If Person is not found, create a new one
                    if (person == null)
                    {
                        person = new Person
                        {
                            Name = doctorDto.Name,
                            Surname = doctorDto.Surname,
                            Gender = doctorDto.Gender
                        };
                        await _context.People.AddAsync(person);
                        await _context.SaveChangesAsync(); // Save new Person to get the ID
                    }
                }
                else
                {
                    // Create new Person if no ID was provided
                    person = new Person
                    {
                        Name = doctorDto.Name,
                        Surname = doctorDto.Surname,
                        Gender = doctorDto.Gender
                    };
                    await _context.People.AddAsync(person);
                    await _context.SaveChangesAsync(); // Save new Person to get the ID
                }

                // Create Doctor linked to the found or newly created Person
                var newDoctor = new Doctor
                {
                    PersonId = person.Id
                };

                await _context.Doctors.AddAsync(newDoctor);
                await _context.SaveChangesAsync();

                // Return a properly structured DTO
                return new DoctorReadDto
                {
                    Id = newDoctor.Id,
                    PersonId = newDoctor.PersonId,
                    Name = person.Name,
                    Surname = person.Surname,
                    Gender = person.Gender
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating doctor", ex);
            }
        }

        // ✅ PUT: Update an existing doctor
        public async Task<bool> UpdateDoctorAsync(DoctorUpdateDto doctorDto)
        {
            try
            {
                var doctor = await _context.Doctors
                    .Include(d => d.Person)
                    .FirstOrDefaultAsync(d => d.Id == doctorDto.Id);

                if (doctor == null) return false;

                // Update fields if they are provided
                if (!string.IsNullOrEmpty(doctorDto.Name)) doctor.Person.Name = doctorDto.Name;
                if (!string.IsNullOrEmpty(doctorDto.Surname)) doctor.Person.Surname = doctorDto.Surname;
                if (!string.IsNullOrEmpty(doctorDto.Gender)) doctor.Person.Gender = doctorDto.Gender;
              

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating doctor with ID {doctorDto.Id}", ex);
            }
        }

        // ✅ DELETE: Remove a doctor by ID
        public async Task<bool> DeleteDoctorAsync(int id)
        {
            try
            {
                var doctor = await _context.Doctors.FindAsync(id);
                if (doctor == null) return false;

                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting doctor with ID {id}", ex);
            }
        }
        public async Task<List<DoctorSummaryDto>> GetDoctorSummariesAsync()
        {
            var doctors = await _context.Doctors
                .Include(dc=>dc.Person)                
    .Include(d => d.DoctorsLicences)
        .ThenInclude(dl => dl.Licence)
        .ThenInclude(li => li.Practices) // Remove `.ThenInclude(pr => pr.Practice)`
    .ToListAsync();

            DateTime referenceDate = DateTime.Now; // Start checking from today

            var doctorSummaries = new List<DoctorSummaryDto>();

            foreach (var doctor in doctors)
            {
                // 1️⃣ Get active licenses
                var activeLicenses = doctor.DoctorsLicences?
                    .Where(dl => dl.Licence.ExpirationDate > DateTime.Now)
                    .Select(dl => new LicenceDto
                    {
                        Id = dl.Licence.Id,
                        IssueDate = dl.Licence.IssueDate,
                        ExpirationDate = dl.Licence.ExpirationDate,
                        PracticeName = dl.Licence.Practices.Practice
                    })
                    .ToList() ?? new List<LicenceDto>();

                // 2️⃣ Retrieve all future appointments for this doctor
                var appointments = await _context.Appointments
                    .Where(a => a.Doctor == doctor.Id && a.StartTime >= DateTime.Now)
                    .ToListAsync();

                DateTime? nextAvailableSlot = null;

                // 3️⃣ Check availability for the next 30 days
                for (int i = 0; i < 30; i++)
                {
                    DateTime checkDate = referenceDate.AddDays(i);
                    List<AppointmentSlot> availableSlots = GetAvailableTimeSlots(appointments, checkDate);

                    if (availableSlots.Any())
                    {
                        nextAvailableSlot = availableSlots.First().StartTime;
                        break; // Stop once we find an available slot
                    }
                }

                // 4️⃣ Create doctor summary
                doctorSummaries.Add(new DoctorSummaryDto
                {
                    DoctorId = doctor.Id,
                    Name = doctor.Person.Name,
                    Surname = doctor.Person.Surname,
                    ActiveLicenses = activeLicenses,
                    NextAvailableSlot = nextAvailableSlot
                });
            }

            return doctorSummaries;
        }
        private List<AppointmentSlot> GetAvailableTimeSlots(List<Appointment> appointments, DateTime referenceDate)
        {
            List<AppointmentSlot> availableSlots = new List<AppointmentSlot>();

            // Define working hours based on provided reference date
            DateTime workStart = referenceDate.Date.AddHours(9);  // 9:00 AM
            DateTime workEnd = referenceDate.Date.AddHours(18);   // 6:00 PM

            // If no appointments exist, return the whole workday as available
            if (!appointments.Any())
            {
                availableSlots.Add(new AppointmentSlot { StartTime = workStart, EndTime = workEnd });
                return SplitIntoThirtyMinuteIntervals(availableSlots);
            }

            // Sort appointments by start time
            var sortedAppointments = appointments.OrderBy(a => a.StartTime).ToList();
            DateTime currentCheck = workStart;

            // ✅ Check gaps between appointments
            foreach (var appointment in sortedAppointments)
            {
                if (currentCheck < appointment.StartTime)
                {
                    availableSlots.Add(new AppointmentSlot { StartTime = currentCheck, EndTime = appointment.StartTime });
                }
                currentCheck = appointment.EndTime ?? appointment.StartTime.AddMinutes(30);
            }

            // ✅ Ensure the final available slot reaches **exactly** 6:00 PM
            if (currentCheck < workEnd)
            {
                availableSlots.Add(new AppointmentSlot { StartTime = currentCheck, EndTime = workEnd });
            }

            return SplitIntoThirtyMinuteIntervals(availableSlots); // ✅ Split into 30-minute intervals
        }





        private List<AppointmentSlot> SplitIntoThirtyMinuteIntervals(List<AppointmentSlot> slots)
        {
            List<AppointmentSlot> splitSlots = new List<AppointmentSlot>();

            foreach (var slot in slots)
            {
                DateTime start = slot.StartTime;
                DateTime end = slot.EndTime ?? start.AddMinutes(30);

                while (start < end)
                {
                    DateTime nextSlotEnd = start.AddMinutes(30);
                    if (nextSlotEnd > end) nextSlotEnd = end; // Prevent exceeding time range

                    splitSlots.Add(new AppointmentSlot { StartTime = start, EndTime = nextSlotEnd });
                    start = nextSlotEnd;
                }
            }

            return splitSlots;
        }

      








    }
}
