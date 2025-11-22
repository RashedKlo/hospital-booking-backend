using Microsoft.AspNetCore.Mvc;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Services.Interfaces;
using hospital_booking.Api.Responses;

namespace hospital_booking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly ILogger<DoctorController> _logger;

        public DoctorController(IDoctorService doctorService, ILogger<DoctorController> logger)
        {
            _doctorService = doctorService;
            _logger = logger;
        }

        /// <summary>
        /// Get doctor by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctor(int id)
        {
            _logger.LogInformation("Getting doctor by ID: {DoctorId}", id);

            var result = await _doctorService.GetDoctorAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to get doctor {DoctorId}: {Message}", id, result.Message);
                return NotFound(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            return Ok(new SuccessResponse<DoctorDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Get all doctors with pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDoctors([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            _logger.LogInformation("Getting doctors - Page: {Page}, Limit: {Limit}", page, limit);

            var result = await _doctorService.GetDoctorsAsync(page, limit);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to get doctors: {Message}", result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            return Ok(new SuccessResponse<List<DoctorDto>>(result.Data!, result.Message));
        }

        /// <summary>
        /// Create a new doctor
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateDoctor([FromBody] DoctorDto dto)
        {
            _logger.LogInformation("Creating doctor: {FullName}", dto.FullName);

            var result = await _doctorService.CreateDoctorAsync(dto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create doctor: {Message}", result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Doctor created successfully - DoctorId: {DoctorId}", result.Data?.DoctorId);
            return CreatedAtAction(nameof(GetDoctor), new { id = result.Data?.DoctorId }, new SuccessResponse<DoctorDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Update an existing doctor
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] DoctorDto dto)
        {
            _logger.LogInformation("Updating doctor: {DoctorId}", id);

            var result = await _doctorService.UpdateDoctorAsync(id, dto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update doctor {DoctorId}: {Message}", id, result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Doctor updated successfully - DoctorId: {DoctorId}", result.Data?.DoctorId);
            return Ok(new SuccessResponse<DoctorDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Delete a doctor
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            _logger.LogInformation("Deleting doctor: {DoctorId}", id);

            var result = await _doctorService.DeleteDoctorAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete doctor {DoctorId}: {Message}", id, result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Doctor deleted successfully - DoctorId: {DoctorId}", id);
            return Ok(new SuccessResponse<bool>(result.Data, result.Message));
        }
    }
}
