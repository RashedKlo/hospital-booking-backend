using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Clinic;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.Clinic
{
    public static class ClinicValidation
    {
        public static async Task<OperationResult<bool>> ValidateAddAsync(
            ClinicAddDto dto, 
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("Clinic add data cannot be null");
                return OperationResult<bool>.Failure("Clinic data is required");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                logger.LogError("Clinic name is required");
                return OperationResult<bool>.Failure("Name is required");
            }

            if (string.IsNullOrWhiteSpace(dto.Phone))
            {
                logger.LogError("Clinic phone is required");
                return OperationResult<bool>.Failure("Phone number is required");
            }

            if (string.IsNullOrWhiteSpace(dto.Address))
            {
                logger.LogError("Clinic address is required");
                return OperationResult<bool>.Failure("Address is required");
            }

            return OperationResult<bool>.Success(true);
        }

        public static async Task<OperationResult<bool>> ValidateUpdateAsync(
            int clinicId,
            ClinicUpdateDto dto, 
            IClinicRepository clinicRepository,
            ILogger logger)
        {
            if (clinicId <= 0)
            {
                logger.LogError("Invalid clinic ID: {ClinicId}", clinicId);
                return OperationResult<bool>.Failure("Invalid clinic ID");
            }

            if (dto == null)
            {
                logger.LogError("Clinic update data cannot be null");
                return OperationResult<bool>.Failure("Clinic update data is required");
            }

            // Check if clinic exists
            var existingResult = await clinicRepository.GetClinicAsync(clinicId);
            if (!existingResult.IsSuccess || existingResult.Data == null)
            {
                logger.LogWarning("Clinic with ID {ClinicId} not found for update", clinicId);
                return OperationResult<bool>.Failure($"Clinic with ID {clinicId} does not exist");
            }

            return OperationResult<bool>.Success(true);
        }
    }
}

