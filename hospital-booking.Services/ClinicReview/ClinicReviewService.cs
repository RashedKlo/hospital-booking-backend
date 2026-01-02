using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.ClinicReview;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using hospital_booking.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.ClinicReview
{
    public class ClinicReviewService : IClinicReviewService
    {
        private readonly IClinicReviewRepository _clinicReviewRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<ClinicReviewService> _logger;

        public ClinicReviewService(
            IClinicReviewRepository clinicReviewRepository, 
            IClinicRepository clinicRepository,
            IPatientRepository patientRepository,
            ILogger<ClinicReviewService> logger)
        {
            _clinicReviewRepository = clinicReviewRepository ?? throw new ArgumentNullException(nameof(clinicReviewRepository));
            _clinicRepository = clinicRepository ?? throw new ArgumentNullException(nameof(clinicRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<bool>> CreateReviewAsync(ClinicReviewAddDto dto)
        {
            _logger.LogInformation("Creating review for ClinicId: {ClinicId}, PatientId: {PatientId}", 
                dto?.ClinicId, dto?.PatientId);

            var validationResult = await ClinicReviewValidation.ValidateAddAsync(dto!, _clinicRepository, _patientRepository, _logger);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<bool>.Failure(validationResult.Message);
            }

            return await _clinicReviewRepository.CreateReviewAsync(dto!);
        }

        public async Task<OperationResult<ClinicReviewDto>> UpdateReviewAsync(int reviewId, ClinicReviewUpdateDto dto)
        {
            _logger.LogInformation("Updating review: {ReviewId}", reviewId);

            var validationResult = await ClinicReviewValidation.ValidateUpdateAsync(reviewId, dto, _clinicReviewRepository, _logger);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<ClinicReviewDto>.Failure(validationResult.Message);
            }

            return await _clinicReviewRepository.UpdateReviewAsync(reviewId, dto);
        }


        public async Task<OperationResult<bool>> DeleteReviewAsync(int reviewId)
        {
            return await _clinicReviewRepository.DeleteReviewAsync(reviewId);
        }

        public async Task<OperationResult<ClinicReviewDto>> GetReviewAsync(int reviewId)
        {
            return await _clinicReviewRepository.GetReviewAsync(reviewId);
        }

        public async Task<OperationResult<ClinicReviewsDto>> GetReviewsAsync(ClinicReviewsRequestDto requestDto)
        {
            return await _clinicReviewRepository.GetReviewsAsync(requestDto);
        }
    }
}
