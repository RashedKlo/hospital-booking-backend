using Microsoft.AspNetCore.Mvc;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Services.Interfaces;
using hospital_booking.Api.Responses;
using System.Linq; // For ToList()
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly ILogger<PatientController> _logger;

        public PatientController(IPatientService patientService, ILogger<PatientController> logger)
        {
            _patientService = patientService;
            _logger = logger;
        }

        /// <summary>
        /// Get patient by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatient(int id)
        {
            _logger.LogInformation("Getting patient by ID: {PatientId}", id);

            var result = await _patientService.GetPatientAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to get patient {PatientId}: {Message}", id, result.Message);
                return NotFound(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            return Ok(new SuccessResponse<PatientDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Get all patients with filtering and pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPatients([FromQuery] PatientsRequestDto requestDto)
        {
            _logger.LogInformation("Getting patients - Page: {Page}, Limit: {Limit}", requestDto.Page, requestDto.Limit);

            var result = await _patientService.GetPatientsAsync(requestDto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to get patients: {Message}", result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            return Ok(new SuccessResponse<PatientsDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Create a new patient
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] PatientAddDto dto)
        {
            _logger.LogInformation("Creating patient: {FullName}", dto.FullName);

            var result = await _patientService.CreatePatientAsync(dto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create patient: {Message}", result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Patient created successfully");
            return Ok(new SuccessResponse<bool>(result.Data, result.Message));
        }

        /// <summary>
        /// Update an existing patient
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] PatientUpdateDto dto)
        {
            _logger.LogInformation("Updating patient: {PatientId}", id);

            var result = await _patientService.UpdatePatientAsync(id, dto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update patient {PatientId}: {Message}", id, result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Patient updated successfully - PatientId: {PatientId}", result.Data?.PatientId);
            return Ok(new SuccessResponse<PatientDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Delete a patient
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            _logger.LogInformation("Deleting patient: {PatientId}", id);

            var result = await _patientService.DeletePatientAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete patient {PatientId}: {Message}", id, result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Patient deleted successfully - PatientId: {PatientId}", id);
            return Ok(new SuccessResponse<bool>(result.Data, result.Message));
        }
    }
}
