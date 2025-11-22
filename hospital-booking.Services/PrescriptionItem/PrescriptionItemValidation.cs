using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.PrescriptionItem;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.PrescriptionItem
{
    public class PrescriptionItemValidation
    {
        public static async Task<OperationResult<bool>> ValidatePrescriptionItemAsync(PrescriptionItemDto dto, IPrescriptionItemRepository prescriptionItemRepository, ILogger logger)
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

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                logger.LogError("Medication name is required");
                return OperationResult<bool>.Failure("Medication name is required");
            }

            return OperationResult<bool>.Success(true);
        }
    }
}
