using HealthApp.Server.DTOs;
using HealthApp.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace HealthApp.Server.Controllers
{
    [Route("api/doctors")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly DoctorService _doctorService;

        public DoctorController(DoctorService doctorService)
        {
            _doctorService = doctorService;
        }
        [HttpGet("summaries")]
        public async Task<ActionResult<IEnumerable<DoctorSummaryDto>>> GetDoctorSummaries()
        {
            try
            {
                var doctors = await _doctorService.GetDoctorSummariesAsync();
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // ✅ GET: Retrieve all doctors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorReadDto>>> GetAllDoctors()
        {
            try
            {
                var doctors = await _doctorService.GetAllDoctorsAsync();
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // ✅ GET: Retrieve a doctor by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorReadDto>> GetDoctorById(int id)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(id);
                if (doctor == null)
                    return NotFound($"Doctor with ID {id} not found.");

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // ✅ POST: Create a new doctor
        [HttpPost]
        public async Task<ActionResult<DoctorReadDto>> CreateDoctor([FromBody] DoctorCreateDto doctorDto)
        {
            try
            {
                var newDoctor = await _doctorService.CreateDoctorAsync(doctorDto);
                if (newDoctor == null)
                    return BadRequest("Failed to create doctor.");

                return CreatedAtAction(nameof(GetDoctorById), new { id = newDoctor.Id }, newDoctor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // ✅ PUT: Update an existing doctor
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] DoctorUpdateDto doctorDto)
        {
            try
            {
                doctorDto.Id = id;
                bool success = await _doctorService.UpdateDoctorAsync(doctorDto);
                if (!success)
                    return NotFound($"Doctor with ID {id} not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // ✅ DELETE: Remove a doctor by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            try
            {
                bool success = await _doctorService.DeleteDoctorAsync(id);
                if (!success)
                    return NotFound($"Doctor with ID {id} not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
