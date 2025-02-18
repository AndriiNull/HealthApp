using HealthApp.Server.Services;
using Microsoft.AspNetCore.Mvc;
using HealthApp.Server.Models.DTOs.LicenceDTOs;

namespace HealthApp.Server.Controllers
{
    [Route("api/licences")]
    [ApiController]
    public class LicenceController : ControllerBase
    {
        private readonly LicenceService _licenceService;

        public LicenceController(LicenceService licenceService)
        {
            _licenceService = licenceService;
        }

        // GET: api/licences
        [HttpGet]
        public async Task<IActionResult> GetLicences()
        {
            var result = await _licenceService.GetLicencesAsync();
            if (!result.Success)
                return StatusCode(500, result.Message);

            return Ok(result.Data);
        }

        // GET: api/licences/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLicence(int id)
        {
            var result = await _licenceService.GetLicenceByIdAsync(id);
            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.Data);
        }

        // POST: api/licences
        [HttpPost]
        public async Task<IActionResult> CreateLicence([FromBody] CreateLicenceDto licenceDto)
        {
            if (licenceDto == null)
                return BadRequest("Invalid data");

            var result = await _licenceService.CreateLicenceAsync(licenceDto);
            if (!result.Success)
                return StatusCode(500, result.Message);

            return CreatedAtAction(nameof(GetLicence), new { id = result.LicenceId }, result.LicenceId);
        }

        // POST: api/licences/assign
        [HttpPost("assign")]
        public async Task<IActionResult> AssignLicenceToDoctor([FromBody] AssignLicenceDto assignLicenceDto)
        {
            var result = await _licenceService.AssignLicenceToDoctorAsync(assignLicenceDto);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}
