using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.ClinicFacility;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.ClinicFacility
{
    public static class ClinicFacilityValidation
    {
        public static async Task<OperationResult<bool>> ValidateAddAsync(
            ClinicFacilityAddDto dto, 
            IClinicRepository clinicRepository,
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("ClinicFacility add data cannot be null");
                return OperationResult<bool>.Failure("Facility data is required");
            }

            if (dto.ClinicId <= 0)
            {
                logger.LogError("Invalid clinic ID: {ClinicId}", dto.ClinicId);
                return OperationResult<bool>.Failure("Valid clinic ID is required");
            }

            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                logger.LogError("ClinicFacility title is required");
                return OperationResult<bool>.Failure("Facility title is required");
            }

            // Check if clinic exists
            var clinicResult = await clinicRepository.GetClinicAsync(dto.ClinicId);
            if (!clinicResult.IsSuccess || clinicResult.Data == null)
            {
                logger.LogWarning("Attempted to create facility for non-existent clinic ID: {ClinicId}", dto.ClinicId);
                return OperationResult<bool>.Failure($"Clinic with ID {dto.ClinicId} does not exist");
            }

            return OperationResult<bool>.Success(true);
        }

        public static async Task<OperationResult<bool>> ValidateUpdateAsync(
            int facilityId,
            ClinicFacilityUpdateDto dto, 
            IClinicFacilityRepository facilityRepository,
            ILogger logger)
        {
            if (facilityId <= 0)
            {
                logger.LogError("Invalid facility ID: {FacilityId}", facilityId);
                return OperationResult<bool>.Failure("Invalid facility ID");
            }

            if (dto == null)
            {
                logger.LogError("ClinicFacility update data cannot be null");
                return OperationResult<bool>.Failure("Facility update data is required");
            }

            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                logger.LogError("ClinicFacility title is required for update");
                return OperationResult<bool>.Failure("Facility title is required");
            }

            // Check if facility exists
            var existingResult = await facilityRepository.GetFacilityAsync(facilityId);
            if (!existingResult.IsSuccess || existingResult.Data == null)
            {
                logger.LogWarning("Facility with ID {FacilityId} not found for update", facilityId);
                return OperationResult<bool>.Failure($"Facility with ID {facilityId} does not exist");
            }

            return OperationResult<bool>.Success(true);
        }
    }
}
