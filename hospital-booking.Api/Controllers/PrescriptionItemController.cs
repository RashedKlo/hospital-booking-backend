using Microsoft.AspNetCore.Mvc;
using hospital_booking.Data.DTOs.PrescriptionItem;
using hospital_booking.Services.Interfaces;
using hospital_booking.Api.Responses;

namespace hospital_booking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionItemController : ControllerBase
    {
        private readonly IPrescriptionItemService _prescriptionItemService;
        private readonly ILogger<PrescriptionItemController> _logger;

        public PrescriptionItemController(IPrescriptionItemService prescriptionItemService, ILogger<PrescriptionItemController> logger)
        {
            _prescriptionItemService = prescriptionItemService;
            _logger = logger;
        }

        /// <summary>
        /// Get prescription item by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrescriptionItem(int id)
        {
            _logger.LogInformation("Getting prescription item by ID: {ItemId}", id);

            var result = await _prescriptionItemService.GetPrescriptionItemAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to get prescription item {ItemId}: {Message}", id, result.Message);
                return NotFound(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            return Ok(new SuccessResponse<PrescriptionItemDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Get all prescription items with pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPrescriptionItems([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            _logger.LogInformation("Getting prescription items - Page: {Page}, Limit: {Limit}", page, limit);

            var result = await _prescriptionItemService.GetPrescriptionItemsAsync(page, limit);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to get prescription items: {Message}", result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            return Ok(new SuccessResponse<List<PrescriptionItemDto>>(result.Data!, result.Message));
        }

        /// <summary>
        /// Get prescription items by prescription ID
        /// </summary>
        [HttpGet("prescription/{prescriptionId}")]
        public async Task<IActionResult> GetPrescriptionItemsByPrescription(int prescriptionId)
        {
            _logger.LogInformation("Getting prescription items by PrescriptionId: {PrescriptionId}", prescriptionId);

            var result = await _prescriptionItemService.GetPrescriptionItemsByPrescriptionAsync(prescriptionId);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to get prescription items for prescription {PrescriptionId}: {Message}", prescriptionId, result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            return Ok(new SuccessResponse<List<PrescriptionItemDto>>(result.Data!, result.Message));
        }

        /// <summary>
        /// Create a new prescription item
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePrescriptionItem([FromBody] PrescriptionItemDto dto)
        {
            _logger.LogInformation("Creating prescription item: {Name} for PrescriptionId: {PrescriptionId}", dto.Name, dto.PrescriptionId);

            var result = await _prescriptionItemService.CreatePrescriptionItemAsync(dto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create prescription item: {Message}", result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Prescription item created successfully - ItemId: {ItemId}", result.Data?.ItemId);
            return CreatedAtAction(nameof(GetPrescriptionItem), new { id = result.Data?.ItemId }, new SuccessResponse<PrescriptionItemDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Update an existing prescription item
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescriptionItem(int id, [FromBody] PrescriptionItemDto dto)
        {
            _logger.LogInformation("Updating prescription item: {ItemId}", id);

            var result = await _prescriptionItemService.UpdatePrescriptionItemAsync(id, dto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update prescription item {ItemId}: {Message}", id, result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Prescription item updated successfully - ItemId: {ItemId}", result.Data?.ItemId);
            return Ok(new SuccessResponse<PrescriptionItemDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Delete a prescription item
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescriptionItem(int id)
        {
            _logger.LogInformation("Deleting prescription item: {ItemId}", id);

            var result = await _prescriptionItemService.DeletePrescriptionItemAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete prescription item {ItemId}: {Message}", id, result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Prescription item deleted successfully - ItemId: {ItemId}", id);
            return Ok(new SuccessResponse<bool>(result.Data, result.Message));
        }
    }
}
