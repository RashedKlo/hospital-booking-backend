using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.Doctor
{
    public class DoctorValidation
    {
        public static async Task<OperationResult<bool>> ValidateDoctorAsync(DoctorDto dto, IDoctorRepository doctorRepository, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("Doctor data cannot be null");
                return OperationResult<bool>.Failure("Doctor data cannot be null");
            }

            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                logger.LogError("Doctor full name is required");
                return OperationResult<bool>.Failure("Full name is required");
            }

            if (dto.ClinicId <= 0)
            {
                logger.LogError("Valid clinic ID is required");
                return OperationResult<bool>.Failure("Valid clinic ID is required");
            }

            return OperationResult<bool>.Success(true);
        }
    }
}
