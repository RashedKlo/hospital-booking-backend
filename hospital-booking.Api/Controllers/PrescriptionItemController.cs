using Microsoft.AspNetCore.Mvc;
using hospital_booking.Data.DTOs.PrescriptionItem;
using hospital_booking.Services.Interfaces;
using hospital_booking.Api.Responses;
using System.Linq; 
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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
        /// Get all prescription items with filtering and pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPrescriptionItems([FromQuery] PrescriptionItemsRequestDto requestDto)
        {
            _logger.LogInformation("Getting prescription items - Page: {Page}, Limit: {Limit}", requestDto.Page, requestDto.Limit);

            var result = await _prescriptionItemService.GetPrescriptionItemsAsync(requestDto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to get prescription items: {Message}", result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            return Ok(new SuccessResponse<PrescriptionItemsDto>(result.Data!, result.Message));
        }

        /// <summary>
        /// Create a new prescription item
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePrescriptionItem([FromBody] PrescriptionItemAddDto dto)
        {
            _logger.LogInformation("Creating prescription item: {Name} for PrescriptionId: {PrescriptionId}", dto.MedicationName, dto.PrescriptionId);

            var result = await _prescriptionItemService.CreatePrescriptionItemAsync(dto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create prescription item: {Message}", result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Prescription item created successfully");
            return Ok(new SuccessResponse<bool>(result.Data, result.Message));
        }

        /// <summary>
        /// Update an existing prescription item
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescriptionItem(int id, [FromBody] PrescriptionItemUpdateDto dto)
        {
            _logger.LogInformation("Updating prescription item: {ItemId}", id);

            var result = await _prescriptionItemService.UpdatePrescriptionItemAsync(id, dto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update prescription item {ItemId}: {Message}", id, result.Message);
                return BadRequest(new ErrorResponse(result.Message, result.Errors.ToList()));
            }

            _logger.LogInformation("Prescription item updated successfully - ItemId: {ItemId}", result.Data?.PrescriptionItemId);
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
