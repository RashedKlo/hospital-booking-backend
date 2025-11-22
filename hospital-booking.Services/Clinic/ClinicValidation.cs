using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Clinic;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.Clinic
{
    public class ClinicValidation
    {
        public static async Task<OperationResult<bool>> ValidateClinicAsync(ClinicDto dto, IClinicRepository clinicRepository, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("Clinic data cannot be null");
                return OperationResult<bool>.Failure("Clinic data cannot be null");
            }

            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                logger.LogError("Clinic title is required");
                return OperationResult<bool>.Failure("Title is required");
            }

            if (string.IsNullOrWhiteSpace(dto.Phone))
            {
                logger.LogError("Clinic phone is required");
                return OperationResult<bool>.Failure("Phone is required");
            }

            if (string.IsNullOrWhiteSpace(dto.Address))
            {
                logger.LogError("Clinic address is required");
                return OperationResult<bool>.Failure("Address is required");
            }

            return OperationResult<bool>.Success(true);
        }
    }
}
