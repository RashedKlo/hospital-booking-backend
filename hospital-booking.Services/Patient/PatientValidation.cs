using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.Patient
{
    public class PatientValidation
    {
        public static async Task<OperationResult<bool>> ValidatePatientAsync(PatientDto dto, IPatientRepository patientRepository, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("Patient data cannot be null");
                return OperationResult<bool>.Failure("Patient data cannot be null");
            }

            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                logger.LogError("Patient full name is required");
                return OperationResult<bool>.Failure("Full name is required");
            }

            return OperationResult<bool>.Success(true);
        }
    }
}
