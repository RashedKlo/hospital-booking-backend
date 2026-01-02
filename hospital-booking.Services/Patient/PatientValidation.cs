using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.Patient
{
    public static class PatientValidation
    {
        public static async Task<OperationResult<bool>> ValidateAddAsync(
            PatientAddDto dto, 
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("Patient add data cannot be null");
                return OperationResult<bool>.Failure("Patient data is required");
            }

            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                logger.LogError("Patient full name is required");
                return OperationResult<bool>.Failure("Full name is required");
            }

            // Optional: If UserId is provided, you might want to validate it here
            // But usually this is handled by database FK constraints or a UserService check

            return OperationResult<bool>.Success(true);
        }

        public static async Task<OperationResult<bool>> ValidateUpdateAsync(
            int patientId,
            PatientUpdateDto dto, 
            IPatientRepository patientRepository,
            ILogger logger)
        {
            if (patientId <= 0)
            {
                logger.LogError("Invalid patient ID: {PatientId}", patientId);
                return OperationResult<bool>.Failure("Invalid patient ID");
            }

            if (dto == null)
            {
                logger.LogError("Patient update data cannot be null");
                return OperationResult<bool>.Failure("Patient update data is required");
            }

            // Check if patient exists
            var existingResult = await patientRepository.GetPatientAsync(patientId);
            if (!existingResult.IsSuccess || existingResult.Data == null)
            {
                logger.LogWarning("Patient with ID {PatientId} not found for update", patientId);
                return OperationResult<bool>.Failure($"Patient with ID {patientId} does not exist");
            }

            return OperationResult<bool>.Success(true);
        }
    }
}

