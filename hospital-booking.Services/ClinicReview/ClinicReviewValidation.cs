using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.ClinicReview;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.ClinicReview
{
    public static class ClinicReviewValidation
    {
        public static async Task<OperationResult<bool>> ValidateAddAsync(
            ClinicReviewAddDto dto, 
            IClinicRepository clinicRepository,
            IPatientRepository patientRepository,
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("ClinicReview add data cannot be null");
                return OperationResult<bool>.Failure("Review data is required");
            }

            if (dto.ClinicId <= 0)
            {
                logger.LogError("Invalid clinic ID: {ClinicId}", dto.ClinicId);
                return OperationResult<bool>.Failure("Valid clinic ID is required");
            }

            if (dto.PatientId <= 0)
            {
                logger.LogError("Invalid patient ID: {PatientId}", dto.PatientId);
                return OperationResult<bool>.Failure("Valid patient ID is required");
            }

            if (dto.Rating < 1 || dto.Rating > 5)
            {
                logger.LogError("Invalid rating: {Rating}", dto.Rating);
                return OperationResult<bool>.Failure("Rating must be between 1 and 5");
            }

            // Check if clinic exists
            var clinicResult = await clinicRepository.GetClinicAsync(dto.ClinicId);
            if (!clinicResult.IsSuccess || clinicResult.Data == null)
            {
                logger.LogWarning("Attempted to create review for non-existent clinic ID: {ClinicId}", dto.ClinicId);
                return OperationResult<bool>.Failure($"Clinic with ID {dto.ClinicId} does not exist");
            }

            // Check if patient exists
            var patientResult = await patientRepository.GetPatientAsync(dto.PatientId);
            if (!patientResult.IsSuccess || patientResult.Data == null)
            {
                logger.LogWarning("Attempted to create review for non-existent patient ID: {PatientId}", dto.PatientId);
                return OperationResult<bool>.Failure($"Patient with ID {dto.PatientId} does not exist");
            }

            return OperationResult<bool>.Success(true);
        }

        public static async Task<OperationResult<bool>> ValidateUpdateAsync(
            int reviewId,
            ClinicReviewUpdateDto dto, 
            IClinicReviewRepository reviewRepository,
            ILogger logger)
        {
            if (reviewId <= 0)
            {
                logger.LogError("Invalid review ID: {ReviewId}", reviewId);
                return OperationResult<bool>.Failure("Invalid review ID");
            }

            if (dto == null)
            {
                logger.LogError("ClinicReview update data cannot be null");
                return OperationResult<bool>.Failure("Review update data is required");
            }

            if (dto.Rating.HasValue && (dto.Rating < 1 || dto.Rating > 5))
            {
                logger.LogError("Invalid rating: {Rating}", dto.Rating);
                return OperationResult<bool>.Failure("Rating must be between 1 and 5");
            }

            // Check if review exists
            var existingResult = await reviewRepository.GetReviewAsync(reviewId);
            if (!existingResult.IsSuccess || existingResult.Data == null)
            {
                logger.LogWarning("Review with ID {ReviewId} not found for update", reviewId);
                return OperationResult<bool>.Failure($"Review with ID {reviewId} does not exist");
            }

            return OperationResult<bool>.Success(true);
        }
    }
}
