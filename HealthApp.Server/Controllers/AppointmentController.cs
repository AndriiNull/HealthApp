using HealthApp.Server.Models.DTOs;
using HealthApp.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentController(AppointmentService appointmentService, UserManager<ApplicationUser> userManager)
        {
            _appointmentService = appointmentService;
            _userManager = userManager;

        }

        // Get All Appointments
        /*[HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentReadDto>>> GetAppointments()
        {
            var appointments = await _appointmentService.GetAppointmentsAsync();
            return Ok(appointments);
        }*/
        
        [AllowAnonymous]
        [HttpGet("availableSlots/{doctorId}")]
        public async Task<ActionResult<IEnumerable<string>>> GetAvailableSlots(int doctorId, [FromQuery] string date)
        {
            try
            {
                if (!DateTime.TryParse(date, out DateTime selectedDate)) // ✅ Convert query string to DateTime
                {
                    return BadRequest("Invalid date format. Please use YYYY-MM-DD.");
                }

                var slots = await _appointmentService.GetAvailableSlotsForDoctorAsync(doctorId, selectedDate);
                return Ok(slots);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        // Get Appointment by ID


        // Create Appointment
        /* [HttpPost]
         public async Task<ActionResult<AppointmentReadDto>> CreateAppointment([FromBody] AppointmentCreateDto dto)
         {
             if (dto == null) return BadRequest("Invalid appointment data");

             var newAppointment = await _appointmentService.CreateAppointmentAsync(dto);
             return CreatedAtAction(nameof(GetAppointment), new { id = newAppointment.Id }, newAppointment);
         } */

        // Update Appointment
        [HttpPost("book")]
        public async Task<IActionResult> Book([FromBody]AppointmentBook booking)
        {
            var result = await _appointmentService.Book(booking);
            return result;
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentUpdateDto dto)
        {
            if (dto == null) return BadRequest("Invalid update data");

            var updated = await _appointmentService.UpdateAppointmentAsync(id, dto);
            if (!updated) return NotFound();

            return NoContent();
        }
        [HttpGet("{date}")]
        public async Task<IActionResult> GetAppointmentsByDate(string date)
        {
            var doctor = await _userManager.GetUserAsync(User);
            if (doctor == null) return Unauthorized();

            try
            {
                var appointments = await _appointmentService.GetAppointmentsByDate(doctor.PersonId, date);
                return Ok(appointments);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }


        // Delete Appointment
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var deleted = await _appointmentService.DeleteAppointmentAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
        [HttpPost("{appointmentId}/comments")]
        public async Task<IActionResult> AddCommentToAppointment(int appointmentId, [FromBody] AddAppointmentCommentDto commentDto)
        {
            var success = await _appointmentService.AddCommentToAppointmentAsync(appointmentId, commentDto);

            if (!success)
                return NotFound($"Appointment with ID {appointmentId} not found.");

            return Ok("Comment added successfully.");
        }
    }
}
