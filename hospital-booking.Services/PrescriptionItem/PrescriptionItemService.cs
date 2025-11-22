using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.PrescriptionItem;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using hospital_booking.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.PrescriptionItem
{
    public sealed class PrescriptionItemService : IPrescriptionItemService
    {
        private readonly IPrescriptionItemRepository _prescriptionItemRepository;
        private readonly ILogger<PrescriptionItemService> _logger;

        public PrescriptionItemService(IPrescriptionItemRepository prescriptionItemRepository, ILogger<PrescriptionItemService> logger)
        {
            _prescriptionItemRepository = prescriptionItemRepository ?? throw new ArgumentNullException(nameof(prescriptionItemRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<PrescriptionItemDto>> GetPrescriptionItemAsync(int itemId)
        {
            _logger.LogInformation("Fetching prescription item by ID: {ItemId}", itemId);

            var result = await _prescriptionItemRepository.GetPrescriptionItemAsync(itemId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch prescription item {ItemId}: {Message}", itemId, result.Message);
                return OperationResult<PrescriptionItemDto>.Failure(result.Message);
            }

            _logger.LogInformation("Prescription item fetched successfully - ItemId: {ItemId}", result.Data?.ItemId);
            return OperationResult<PrescriptionItemDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<List<PrescriptionItemDto>>> GetPrescriptionItemsAsync(int page, int limit)
        {
            _logger.LogInformation("Fetching prescription items - Page: {Page}, Limit: {Limit}", page, limit);

            var result = await _prescriptionItemRepository.GetPrescriptionItemsAsync(page, limit);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch prescription items: {Message}", result.Message);
                return OperationResult<List<PrescriptionItemDto>>.Failure(result.Message);
            }

            _logger.LogInformation("Fetched {Count} prescription items successfully", result.Data?.Count ?? 0);
            return OperationResult<List<PrescriptionItemDto>>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<List<PrescriptionItemDto>>> GetPrescriptionItemsByPrescriptionAsync(int prescriptionId)
        {
            _logger.LogInformation("Fetching prescription items by PrescriptionId: {PrescriptionId}", prescriptionId);

            var result = await _prescriptionItemRepository.GetPrescriptionItemsByPrescriptionAsync(prescriptionId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch prescription items for prescription {PrescriptionId}: {Message}", prescriptionId, result.Message);
                return OperationResult<List<PrescriptionItemDto>>.Failure(result.Message);
            }

            _logger.LogInformation("Fetched {Count} prescription items for PrescriptionId: {PrescriptionId}", result.Data?.Count ?? 0, prescriptionId);
            return OperationResult<List<PrescriptionItemDto>>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<PrescriptionItemDto>> CreatePrescriptionItemAsync(PrescriptionItemDto prescriptionItemDto)
        {
            if (prescriptionItemDto == null)
            {
                _logger.LogWarning("Create prescription item attempted with null data");
                return OperationResult<PrescriptionItemDto>.Failure("Prescription item data is required");
            }

            _logger.LogInformation("Creating prescription item: {Name} for PrescriptionId: {PrescriptionId}", 
                prescriptionItemDto.Name, prescriptionItemDto.PrescriptionId);

            var result = await _prescriptionItemRepository.CreatePrescriptionItemAsync(prescriptionItemDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create prescription item: {Message}", result.Message);
                return OperationResult<PrescriptionItemDto>.Failure(result.Message);
            }

            _logger.LogInformation("Prescription item created successfully - ItemId: {ItemId}", result.Data?.ItemId);
            return OperationResult<PrescriptionItemDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<PrescriptionItemDto>> UpdatePrescriptionItemAsync(int itemId, PrescriptionItemDto prescriptionItemDto)
        {
            if (prescriptionItemDto == null)
            {
                _logger.LogWarning("Update prescription item attempted with null data");
                return OperationResult<PrescriptionItemDto>.Failure("Prescription item data is required");
            }

            _logger.LogInformation("Updating prescription item: {ItemId}", itemId);

            var result = await _prescriptionItemRepository.UpdatePrescriptionItemAsync(itemId, prescriptionItemDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update prescription item {ItemId}: {Message}", itemId, result.Message);
                return OperationResult<PrescriptionItemDto>.Failure(result.Message);
            }

            _logger.LogInformation("Prescription item updated successfully - ItemId: {ItemId}", result.Data?.ItemId);
            return OperationResult<PrescriptionItemDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<bool>> DeletePrescriptionItemAsync(int itemId)
        {
            _logger.LogInformation("Deleting prescription item: {ItemId}", itemId);

            var result = await _prescriptionItemRepository.DeletePrescriptionItemAsync(itemId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete prescription item {ItemId}: {Message}", itemId, result.Message);
                return OperationResult<bool>.Failure(result.Message);
            }

            _logger.LogInformation("Prescription item deleted successfully - ItemId: {ItemId}", itemId);
            return OperationResult<bool>.Success(result.Data, result.Message);
        }
    }
}
