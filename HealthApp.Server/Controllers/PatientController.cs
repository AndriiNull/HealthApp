using HealthApp.Server.Services;
using Microsoft.AspNetCore.Mvc;
using static HealthApp.Server.Models.DTOs.PatientDTOs.PatientDTOs;

namespace HealthApp.Server.Controllers
{
    [Route("api/patients")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly PatientService _patientService;

        public PatientController(PatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPatients()
        {
            var result = await _patientService.GetPatientsAsync();
            if (!result.Success)
                return StatusCode(500, result.Message);

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatient(int id)
        {
            var result = await _patientService.GetPatientByIdAsync(id);
            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientDto patientDto)
        {
            var result = await _patientService.CreatePatientAsync(patientDto);
            if (!result.Success)
                return StatusCode(500, result.Message);

            return CreatedAtAction(nameof(GetPatient), new { id = result.PatientId }, result.PatientId);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] UpdatePatientDto updateDto)
        {
            var result = await _patientService.UpdatePatientAsync(id, updateDto);
            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var result = await _patientService.DeletePatientAsync(id);
            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.Message);
        }
    }
}
