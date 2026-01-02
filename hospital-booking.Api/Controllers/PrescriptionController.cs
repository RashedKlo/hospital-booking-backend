using Microsoft.AspNetCore.Mvc;
using hospital_booking.Data.DTOs.Prescription;
using hospital_booking.Services.Interfaces;
using hospital_booking.Api.Responses;
using System.Linq; 
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly ILogger<PrescriptionController> _logger;

        public PrescriptionController(IPrescriptionService prescriptionService, ILogger<PrescriptionController> logger)
        {
            _prescriptionService = prescriptionService;
            _logger = logger;
        }

        /// <summary>
        /// Get prescription by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrescription(int id)
        {
            _logger.LogInformation("Getting prescription by ID: {PrescriptionId}", id);

            var result = await _prescriptionService.GetPrescriptionAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to get prescription {PrescriptionId}: {Message}", id, result.Message);
                return NotFound(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            return Ok(new SuccessResponse<PrescriptionDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Get all prescriptions with filtering and pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPrescriptions([FromQuery] PrescriptionsRequestDto requestDto)
        {
            _logger.LogInformation("Getting prescriptions - Page: {Page}, Limit: {Limit}", requestDto.Page, requestDto.Limit);

            var result = await _prescriptionService.GetPrescriptionsAsync(requestDto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to get prescriptions: {Message}", result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            return Ok(new SuccessResponse<PrescriptionsDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Create a new prescription
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePrescription([FromBody] PrescriptionAddDto dto)
        {
            _logger.LogInformation("Creating prescription for AppointmentId: {AppointmentId}", dto.AppointmentId);

            var result = await _prescriptionService.CreatePrescriptionAsync(dto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create prescription: {Message}", result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Prescription created successfully");
            return Ok(new SuccessResponse<bool>(result.Data, result.Message));
        }

        /// <summary>
        /// Update an existing prescription
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescription(int id, [FromBody] PrescriptionUpdateDto dto)
        {
            _logger.LogInformation("Updating prescription: {PrescriptionId}", id);

            var result = await _prescriptionService.UpdatePrescriptionAsync(id, dto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update prescription {PrescriptionId}: {Message}", id, result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Prescription updated successfully - PrescriptionId: {PrescriptionId}", result.Data?.PrescriptionId);
            return Ok(new SuccessResponse<PrescriptionDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Delete a prescription
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            _logger.LogInformation("Deleting prescription: {PrescriptionId}", id);

            var result = await _prescriptionService.DeletePrescriptionAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete prescription {PrescriptionId}: {Message}", id, result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Prescription deleted successfully - PrescriptionId: {PrescriptionId}", id);
            return Ok(new SuccessResponse<bool>(result.Data, result.Message));
        }
    }
}
