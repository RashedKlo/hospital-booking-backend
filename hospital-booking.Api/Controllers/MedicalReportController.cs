using Microsoft.AspNetCore.Mvc;
using hospital_booking.Data.DTOs.MedicalReport;
using hospital_booking.Services.Interfaces;
using hospital_booking.Api.Responses;
using System.Linq; // For ToList()
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalReportController : ControllerBase
    {
        private readonly IMedicalReportService _medicalReportService;
        private readonly ILogger<MedicalReportController> _logger;

        public MedicalReportController(IMedicalReportService medicalReportService, ILogger<MedicalReportController> logger)
        {
            _medicalReportService = medicalReportService;
            _logger = logger;
        }

        /// <summary>
        /// Get medical report by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicalReport(int id)
        {
            _logger.LogInformation("Getting medical report by ID: {ReportId}", id);

            var result = await _medicalReportService.GetMedicalReportAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to get medical report {ReportId}: {Message}", id, result.Message);
                return NotFound(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            return Ok(new SuccessResponse<MedicalReportDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Get all medical reports with filtering and pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMedicalReports([FromQuery] MedicalReportsRequestDto requestDto)
        {
            _logger.LogInformation("Getting medical reports - Page: {Page}, Limit: {Limit}", requestDto.Page, requestDto.Limit);

            var result = await _medicalReportService.GetMedicalReportsAsync(requestDto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to get medical reports: {Message}", result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            return Ok(new SuccessResponse<MedicalReportsDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Create a new medical report
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateMedicalReport([FromBody] MedicalReportAddDto dto)
        {
            _logger.LogInformation("Creating medical report for AppointmentId: {AppointmentId}", dto.AppointmentId);

            var result = await _medicalReportService.CreateMedicalReportAsync(dto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create medical report: {Message}", result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Medical report created successfully");
            return Ok(new SuccessResponse<bool>(result.Data, result.Message));
        }

        /// <summary>
        /// Update an existing medical report
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicalReport(int id, [FromBody] MedicalReportUpdateDto dto)
        {
            _logger.LogInformation("Updating medical report: {ReportId}", id);

            var result = await _medicalReportService.UpdateMedicalReportAsync(id, dto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update medical report {ReportId}: {Message}", id, result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Medical report updated successfully - ReportId: {ReportId}", result.Data?.ReportId);
            return Ok(new SuccessResponse<MedicalReportDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Delete a medical report
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalReport(int id)
        {
            _logger.LogInformation("Deleting medical report: {ReportId}", id);

            var result = await _medicalReportService.DeleteMedicalReportAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete medical report {ReportId}: {Message}", id, result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Medical report deleted successfully - ReportId: {ReportId}", id);
            return Ok(new SuccessResponse<bool>(result.Data, result.Message));
        }
    }
}
