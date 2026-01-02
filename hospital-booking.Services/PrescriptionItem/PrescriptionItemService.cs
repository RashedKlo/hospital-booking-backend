using System;
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
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly ILogger<PrescriptionItemService> _logger;

        public PrescriptionItemService(
            IPrescriptionItemRepository prescriptionItemRepository,
            IPrescriptionRepository prescriptionRepository,
            ILogger<PrescriptionItemService> logger)
        {
            _prescriptionItemRepository = prescriptionItemRepository ?? throw new ArgumentNullException(nameof(prescriptionItemRepository));
            _prescriptionRepository = prescriptionRepository ?? throw new ArgumentNullException(nameof(prescriptionRepository));
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

            _logger.LogInformation("Prescription item fetched successfully - ItemId: {ItemId}", result.Data?.PrescriptionItemId);
            return OperationResult<PrescriptionItemDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<PrescriptionItemsDto>> GetPrescriptionItemsAsync(PrescriptionItemsRequestDto requestDto)
        {
            _logger.LogInformation("Fetching prescription items - Page: {Page}, Limit: {Limit}", requestDto.Page, requestDto.Limit);

            var result = await _prescriptionItemRepository.GetPrescriptionItemsAsync(requestDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch prescription items: {Message}", result.Message);
                return OperationResult<PrescriptionItemsDto>.Failure(result.Message);
            }

            return OperationResult<PrescriptionItemsDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<bool>> CreatePrescriptionItemAsync(PrescriptionItemAddDto dto)
        {
            _logger.LogInformation("Creating prescription item: {Name} for PrescriptionId: {PrescriptionId}", dto?.MedicationName, dto?.PrescriptionId);

            var validationResult = await PrescriptionItemValidation.ValidateAddAsync(dto!, _prescriptionRepository, _logger);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<bool>.Failure(validationResult.Message);
            }

            var result = await _prescriptionItemRepository.CreatePrescriptionItemAsync(dto!);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create prescription item: {Message}", result.Message);
                return OperationResult<bool>.Failure(result.Message);
            }

            _logger.LogInformation("Prescription item created successfully");
            return OperationResult<bool>.Success(true, result.Message);
        }

        public async Task<OperationResult<PrescriptionItemDto>> UpdatePrescriptionItemAsync(int itemId, PrescriptionItemUpdateDto dto)
        {
            _logger.LogInformation("Updating prescription item: {ItemId}", itemId);

            var validationResult = await PrescriptionItemValidation.ValidateUpdateAsync(itemId, dto, _prescriptionItemRepository, _logger);

            if (!validationResult.IsSuccess)
            {
                return OperationResult<PrescriptionItemDto>.Failure(validationResult.Message);
            }

            var result = await _prescriptionItemRepository.UpdatePrescriptionItemAsync(itemId, dto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update prescription item {ItemId}: {Message}", itemId, result.Message);
                return OperationResult<PrescriptionItemDto>.Failure(result.Message);
            }

            _logger.LogInformation("Prescription item updated successfully - ItemId: {ItemId}", result.Data?.PrescriptionItemId);
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
