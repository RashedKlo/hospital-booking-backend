using Microsoft.AspNetCore.Mvc;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Services.Interfaces;
using hospital_booking.Api.Responses;

namespace hospital_booking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        /// <summary>
        /// Get admin by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdmin(int id)
        {
            _logger.LogInformation("Getting admin by ID: {AdminId}", id);

            var result = await _adminService.GetAdminAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to get admin {AdminId}: {Message}", id, result.Message);
                return NotFound(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            return Ok(new SuccessResponse<AdminDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Get all admins with pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAdmins([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            _logger.LogInformation("Getting admins - Page: {Page}, Limit: {Limit}", page, limit);

            var result = await _adminService.GetAdminsAsync(page, limit);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to get admins: {Message}", result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            return Ok(new SuccessResponse<List<AdminDto>>(result.Data!, result.Message));
        }

        /// <summary>
        /// Create a new admin
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAdmin([FromBody] AdminDto dto)
        {
            _logger.LogInformation("Creating admin: {Email}", dto.Email);

            var result = await _adminService.CreateAdminAsync(dto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create admin: {Message}", result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Admin created successfully - AdminId: {AdminId}", result.Data?.AdminId);
            return CreatedAtAction(nameof(GetAdmin), new { id = result.Data?.AdminId }, new SuccessResponse<AdminDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Update an existing admin
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdmin(int id, [FromBody] AdminDto dto)
        {
            _logger.LogInformation("Updating admin: {AdminId}", id);

            var result = await _adminService.UpdateAdminAsync(id, dto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update admin {AdminId}: {Message}", id, result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Admin updated successfully - AdminId: {AdminId}", result.Data?.AdminId);
            return Ok(new SuccessResponse<AdminDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Delete an admin
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            _logger.LogInformation("Deleting admin: {AdminId}", id);

            var result = await _adminService.DeleteAdminAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete admin {AdminId}: {Message}", id, result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Admin deleted successfully - AdminId: {AdminId}", id);
            return Ok(new SuccessResponse<bool>(result.Data, result.Message));
        }
    }
}
