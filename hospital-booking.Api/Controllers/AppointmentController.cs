using Microsoft.AspNetCore.Mvc;
using hospital_booking.Data.DTOs.Appointment;
using hospital_booking.Services.Interfaces;
using hospital_booking.Api.Responses;

namespace hospital_booking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(IAppointmentService appointmentService, ILogger<AppointmentController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        /// <summary>
        /// Get appointment by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointment(int id)
        {
            _logger.LogInformation("Getting appointment by ID: {AppointmentId}", id);

            var result = await _appointmentService.GetAppointmentAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to get appointment {AppointmentId}: {Message}", id, result.Message);
                return NotFound(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            return Ok(new SuccessResponse<AppointmentDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Get all appointments with pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAppointments([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            _logger.LogInformation("Getting appointments - Page: {Page}, Limit: {Limit}", page, limit);

            var result = await _appointmentService.GetAppointmentsAsync(page, limit);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to get appointments: {Message}", result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            return Ok(new SuccessResponse<List<AppointmentDto>>(result.Data!, result.Message));
        }

        /// <summary>
        /// Create a new appointment
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentDto dto)
        {
            _logger.LogInformation("Creating appointment for PatientId: {PatientId}, DoctorId: {DoctorId}", dto.PatientId, dto.DoctorId);

            var result = await _appointmentService.CreateAppointmentAsync(dto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create appointment: {Message}", result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Appointment created successfully - AppointmentId: {AppointmentId}", result.Data?.AppointmentId);
            return CreatedAtAction(nameof(GetAppointment), new { id = result.Data?.AppointmentId }, new SuccessResponse<AppointmentDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Update an existing appointment
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentDto dto)
        {
            _logger.LogInformation("Updating appointment: {AppointmentId}", id);

            var result = await _appointmentService.UpdateAppointmentAsync(id, dto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update appointment {AppointmentId}: {Message}", id, result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Appointment updated successfully - AppointmentId: {AppointmentId}", result.Data?.AppointmentId);
            return Ok(new SuccessResponse<AppointmentDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Delete an appointment
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            _logger.LogInformation("Deleting appointment: {AppointmentId}", id);

            var result = await _appointmentService.DeleteAppointmentAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete appointment {AppointmentId}: {Message}", id, result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Appointment deleted successfully - AppointmentId: {AppointmentId}", id);
            return Ok(new SuccessResponse<bool>(result.Data, result.Message));
        }
    }
}
