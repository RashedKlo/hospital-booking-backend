using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.PrescriptionItem;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.PrescriptionItem
{
    public static class PrescriptionItemValidation
    {
        public static async Task<OperationResult<bool>> ValidateAddAsync(
            PrescriptionItemAddDto dto, 
            IPrescriptionRepository prescriptionRepository, 
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("Prescription item data cannot be null");
                return OperationResult<bool>.Failure("Prescription item data cannot be null");
            }

            if (dto.PrescriptionId <= 0)
            {
                logger.LogError("Valid prescription ID is required");
                return OperationResult<bool>.Failure("Valid prescription ID is required");
            }

            if (string.IsNullOrWhiteSpace(dto.MedicationName))
            {
                logger.LogError("Medication name is required");
                return OperationResult<bool>.Failure("Medication name is required");
            }

            // Check if prescription exists
            var prescriptionResult = await prescriptionRepository.GetPrescriptionAsync(dto.PrescriptionId);
            if (!prescriptionResult.IsSuccess || prescriptionResult.Data == null)
            {
                logger.LogWarning("Attempted to add item to non-existent prescription ID: {PrescriptionId}", dto.PrescriptionId);
                return OperationResult<bool>.Failure($"Prescription with ID {dto.PrescriptionId} does not exist");
            }

            return OperationResult<bool>.Success(true);
        }

        public static async Task<OperationResult<bool>> ValidateUpdateAsync(
            int itemId,
            PrescriptionItemUpdateDto dto, 
            IPrescriptionItemRepository prescriptionItemRepository,
            ILogger logger)
        {
            if (itemId <= 0)
            {
                logger.LogError("Invalid item ID: {ItemId}", itemId);
                return OperationResult<bool>.Failure("Invalid item ID");
            }

            if (dto == null)
            {
                logger.LogError("Prescription item update data cannot be null");
                return OperationResult<bool>.Failure("Prescription item update data cannot be null");
            }

            // Check if item exists
            var existingItem = await prescriptionItemRepository.GetPrescriptionItemAsync(itemId);
            if (!existingItem.IsSuccess || existingItem.Data == null)
            {
                logger.LogWarning("Prescription item with ID {ItemId} not found for update", itemId);
                return OperationResult<bool>.Failure($"Prescription item with ID {itemId} does not exist");
            }

            // Additional business rules for update can be added here
            if (dto.MedicationName != null && string.IsNullOrWhiteSpace(dto.MedicationName))
            {
                logger.LogError("Medication name cannot be empty if provided");
                return OperationResult<bool>.Failure("Medication name cannot be empty");
            }

            return OperationResult<bool>.Success(true);
        }

    }
}

