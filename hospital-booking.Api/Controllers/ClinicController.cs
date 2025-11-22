using Microsoft.AspNetCore.Mvc;
using hospital_booking.Data.DTOs.Clinic;
using hospital_booking.Services.Interfaces;
using hospital_booking.Api.Responses;

namespace hospital_booking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicController : ControllerBase
    {
        private readonly IClinicService _clinicService;
        private readonly ILogger<ClinicController> _logger;

        public ClinicController(IClinicService clinicService, ILogger<ClinicController> logger)
        {
            _clinicService = clinicService;
            _logger = logger;
        }

        /// <summary>
        /// Get clinic by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClinic(int id)
        {
            _logger.LogInformation("Getting clinic by ID: {ClinicId}", id);

            var result = await _clinicService.GetClinicAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to get clinic {ClinicId}: {Message}", id, result.Message);
                return NotFound(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            return Ok(new SuccessResponse<ClinicDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Get all clinics with pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetClinics([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            _logger.LogInformation("Getting clinics - Page: {Page}, Limit: {Limit}", page, limit);

            var result = await _clinicService.GetClinicsAsync(page, limit);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to get clinics: {Message}", result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            return Ok(new SuccessResponse<List<ClinicDto>>(result.Data!, result.Message));
        }

        /// <summary>
        /// Create a new clinic
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateClinic([FromBody] ClinicDto dto)
        {
            _logger.LogInformation("Creating clinic: {Title}", dto.Title);

            var result = await _clinicService.CreateClinicAsync(dto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create clinic: {Message}", result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Clinic created successfully - ClinicId: {ClinicId}", result.Data?.ClinicId);
            return CreatedAtAction(nameof(GetClinic), new { id = result.Data?.ClinicId }, new SuccessResponse<ClinicDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Update an existing clinic
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClinic(int id, [FromBody] ClinicDto dto)
        {
            _logger.LogInformation("Updating clinic: {ClinicId}", id);

            var result = await _clinicService.UpdateClinicAsync(id, dto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update clinic {ClinicId}: {Message}", id, result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Clinic updated successfully - ClinicId: {ClinicId}", result.Data?.ClinicId);
            return Ok(new SuccessResponse<ClinicDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Delete a clinic
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClinic(int id)
        {
            _logger.LogInformation("Deleting clinic: {ClinicId}", id);

            var result = await _clinicService.DeleteClinicAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete clinic {ClinicId}: {Message}", id, result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Clinic deleted successfully - ClinicId: {ClinicId}", id);
            return Ok(new SuccessResponse<bool>(result.Data, result.Message));
        }
    }
}
