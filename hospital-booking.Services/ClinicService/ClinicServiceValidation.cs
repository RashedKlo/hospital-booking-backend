using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.ClinicService;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.ClinicService
{
    public static class ClinicServiceValidation
    {
        public static async Task<OperationResult<bool>> ValidateAddAsync(
            ClinicServiceAddDto dto, 
            IClinicRepository clinicRepository,
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("ClinicService add data cannot be null");
                return OperationResult<bool>.Failure("Service data is required");
            }

            if (dto.ClinicId <= 0)
            {
                logger.LogError("Invalid clinic ID: {ClinicId}", dto.ClinicId);
                return OperationResult<bool>.Failure("Valid clinic ID is required");
            }

            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                logger.LogError("ClinicService title is required");
                return OperationResult<bool>.Failure("Service title is required");
            }

            if (dto.Price < 0)
            {
                logger.LogError("Invalid price: {Price}", dto.Price);
                return OperationResult<bool>.Failure("Price cannot be negative");
            }

            // Check if clinic exists
            var clinicResult = await clinicRepository.GetClinicAsync(dto.ClinicId);
            if (!clinicResult.IsSuccess || clinicResult.Data == null)
            {
                logger.LogWarning("Attempted to create service for non-existent clinic ID: {ClinicId}", dto.ClinicId);
                return OperationResult<bool>.Failure($"Clinic with ID {dto.ClinicId} does not exist");
            }

            return OperationResult<bool>.Success(true);
        }

        public static async Task<OperationResult<bool>> ValidateUpdateAsync(
            int serviceId,
            ClinicServiceUpdateDto dto, 
            IClinicServiceRepository serviceRepository,
            ILogger logger)
        {
            if (serviceId <= 0)
            {
                logger.LogError("Invalid service ID: {ServiceId}", serviceId);
                return OperationResult<bool>.Failure("Invalid service ID");
            }

            if (dto == null)
            {
                logger.LogError("ClinicService update data cannot be null");
                return OperationResult<bool>.Failure("Service update data is required");
            }

            if (dto.Price.HasValue && dto.Price < 0)
            {
                logger.LogError("Invalid price: {Price}", dto.Price);
                return OperationResult<bool>.Failure("Price cannot be negative");
            }

            // Check if service exists
            var existingResult = await serviceRepository.GetServiceAsync(serviceId);
            if (!existingResult.IsSuccess || existingResult.Data == null)
            {
                logger.LogWarning("Service with ID {ServiceId} not found for update", serviceId);
                return OperationResult<bool>.Failure($"Service with ID {serviceId} does not exist");
            }

            return OperationResult<bool>.Success(true);
        }
    }
}
