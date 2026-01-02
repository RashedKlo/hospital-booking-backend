using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.Doctor
{
    public static class DoctorValidation
    {
        public static async Task<OperationResult<bool>> ValidateAddAsync(
            DoctorAddDto dto, 
            IClinicRepository clinicRepository, 
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("Doctor add data cannot be null");
                return OperationResult<bool>.Failure("Doctor data is required");
            }

            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                logger.LogError("Doctor full name is required");
                return OperationResult<bool>.Failure("Full name is required");
            }

            if (dto.ClinicId <= 0)
            {
                logger.LogError("Invalid clinic ID: {ClinicId}", dto.ClinicId);
                return OperationResult<bool>.Failure("Valid clinic ID is required");
            }

            // Check if clinic exists
            var clinicResult = await clinicRepository.GetClinicAsync(dto.ClinicId);
            if (!clinicResult.IsSuccess || clinicResult.Data == null)
            {
                logger.LogWarning("Attempted to create doctor for non-existent clinic ID: {ClinicId}", dto.ClinicId);
                return OperationResult<bool>.Failure($"Clinic with ID {dto.ClinicId} does not exist");
            }

            return OperationResult<bool>.Success(true);
        }

        public static async Task<OperationResult<bool>> ValidateUpdateAsync(
            int doctorId,
            DoctorUpdateDto dto, 
            IDoctorRepository doctorRepository,
            ILogger logger)
        {
            if (doctorId <= 0)
            {
                logger.LogError("Invalid doctor ID: {DoctorId}", doctorId);
                return OperationResult<bool>.Failure("Invalid doctor ID");
            }

            if (dto == null)
            {
                logger.LogError("Doctor update data cannot be null");
                return OperationResult<bool>.Failure("Doctor update data is required");
            }

            // Check if doctor exists
            var existingResult = await doctorRepository.GetDoctorAsync(doctorId);
            if (!existingResult.IsSuccess || existingResult.Data == null)
            {
                logger.LogWarning("Doctor with ID {DoctorId} not found for update", doctorId);
                return OperationResult<bool>.Failure($"Doctor with ID {doctorId} does not exist");
            }

            return OperationResult<bool>.Success(true);
        }
    }
}

