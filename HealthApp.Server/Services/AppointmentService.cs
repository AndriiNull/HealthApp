using HealthApp.Server.Models;
using HealthApp.Server.Models.DatabaseModels;
using HealthApp.Server.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Server.Services
{
    public class AppointmentService
    {
        private readonly MasterContext _context;

        public AppointmentService(MasterContext context)
        {
            _context = context;
        }
        public async Task<bool> UpdateAppointmentAsync(int id, AppointmentUpdateDto dto)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null) return false;

                appointment.StartTime = dto.StartTime;
                appointment.EndTime = dto.EndTime;
                appointment.Status = dto.Status;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating appointment with ID {id}", ex);
            }
        }

        // Delete Appointment
        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null) return false;

                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting appointment with ID {id}", ex);
            }
        }

        public async Task<List<AppointmentDto>> GetAppointmentsByDate(int personId, string date)
        {
            if (!DateTime.TryParse(date, out DateTime selectedDate))
            {
                throw new ArgumentException("Invalid date format");
            }
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.PersonId == personId);
            var appointments = await _context.Appointments
                .Where(a => a.Doctor == doctor.Id && a.StartTime.Date == selectedDate.Date)
                .Include(a => a.IssueNavigation)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    AppointmentDate = a.StartTime,
                    PatientId = a.IssueNavigation.Patient,
                    PatientName = a.IssueNavigation.PatientNavigation.PersonNavigation.Name,    
                    IssueId = a.Issue
                })
                .ToListAsync();

            return appointments;
        }
        public async Task<bool> AddCommentToAppointmentAsync(int appointmentId, AddAppointmentCommentDto commentDto)
        {
            try
            {
                var appointment = await _context.Appointments
                    .Include(a => a.Comments) // Ensure comments are loaded
                    .FirstOrDefaultAsync(a => a.Id == appointmentId);

                if (appointment == null)
                {
                    return false; // Indicate failure if appointment is not found
                }

                var newComment = new AppointmentComment
                {
                    CommentText = commentDto.CommentText,
                    CreatedAt = DateTime.UtcNow,
                    AppointmentId = appointmentId // Foreign key reference
                };

                _context.AppointmentComments.Add(newComment);
                await _context.SaveChangesAsync();

                return true; // Indicate success
            }
            catch
            {
                return false; // Handle unexpected errors
            }
        }

        public async Task<List<string>> GetAvailableSlotsForDoctorAsync(int doctorId, DateTime selectedDate)
        {
            var localTimeZone = TimeZoneInfo.Local; // ✅ Get server's local timezone

            var bookedAppointments = await _context.Appointments
                .Where(a => a.Doctor == doctorId)
                .Select(a => a.StartTime)
                .ToListAsync();

            // ✅ Convert all booked appointments from UTC to Local before filtering
            var localBookedAppointments = bookedAppointments
                .Select(utcTime => TimeZoneInfo.ConvertTimeFromUtc(utcTime, localTimeZone))
                .Where(a => a.Date == selectedDate.Date) // ✅ Filter in correct timezone
                .ToList();

            // ✅ Generate available slots in Local Time
            var availableSlots = GenerateAvailableTimeSlots(selectedDate, localBookedAppointments);

            // ✅ Convert available slots back to UTC before returning
            return availableSlots.Select(localSlot =>
                TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(localSlot)).ToString("yyyy-MM-dd HH:mm")
            ).ToList();
        }

        private List<string> GenerateAvailableTimeSlots(DateTime selectedDate, List<DateTime> bookedAppointments)
        {
            var localTimeZone = TimeZoneInfo.Local; // ✅ Ensure local timezone consistency
            var startTime = TimeZoneInfo.ConvertTimeFromUtc(selectedDate.Date.AddHours(8), localTimeZone);
            var endTime = TimeZoneInfo.ConvertTimeFromUtc(selectedDate.Date.AddHours(17), localTimeZone);

            var bookedSlots = bookedAppointments.Select(b => b.ToString("HH:mm")).ToHashSet(); // ✅ Compare only times
            var availableSlots = new List<string>();

            var slot = startTime;
            while (slot < endTime)
            {
                string formattedTime = slot.ToString("yyyy-MM-dd HH:mm"); // ✅ Keep full datetime format
                if (!bookedSlots.Contains(slot.ToString("HH:mm")))
                {
                    availableSlots.Add(formattedTime);
                }
                slot = slot.AddMinutes(30);
            }

            return availableSlots;
        }




        public async Task<IActionResult> Book(AppointmentBook booking)
        {
            // ✅ Ensure StartTime is in UTC
            booking.StartTime = booking.StartTime.ToLocalTime();

            // ✅ Check for overlapping appointments in UTC
            if (await HasOverlappingAppointment(booking.DoctorID, booking.StartTime, 30))
            {
                return new BadRequestObjectResult("❌ The doctor already has an appointment at this time.");
            }

            var issue = await _context.Issues.FirstOrDefaultAsync(e => e.Id == booking.IssueId);
            if (issue == null)
                return new BadRequestObjectResult("❌ Error fetching issue.");

            var appointment = new Appointment()
            {
                Issue = booking.IssueId,
                Doctor = booking.DoctorID,
                StartTime = booking.StartTime, // ✅ Now stored as UTC
                EndTime = booking.StartTime.AddMinutes(30),
                Status = "Pending"
            };

            // ✅ Set Lead Doctor if not assigned
            if (issue.LeadDoctor == null)
                issue.LeadDoctor = booking.DoctorID;

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            return new OkObjectResult(appointment.Id);
        }


        /** ✅ Check if the doctor has overlapping appointments */
        public async Task<bool> HasOverlappingAppointment(int doctorId, DateTime requestedStartTime, int durationMinutes = 30)
        {
            var requestedEndTime = requestedStartTime.AddMinutes(durationMinutes);

            var overlappingAppointment = await _context.Appointments
                .Where(a => a.Doctor == doctorId)
                .Where(a => a.StartTime < requestedEndTime && a.StartTime.AddMinutes(durationMinutes) > requestedStartTime)
                .FirstOrDefaultAsync();

            return overlappingAppointment != null;
        }
    }

}

