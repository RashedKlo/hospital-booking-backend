using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Clinic;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using hospital_booking.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.Clinic
{
    public sealed class ClinicService : IClinicService
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IClinicReviewRepository _reviewRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IClinicFacilityRepository _facilityRepository;
        private readonly IClinicServiceRepository _clinicServiceRepository;
        private readonly ILogger<ClinicService> _logger;

        public ClinicService(
            IClinicRepository clinicRepository, 
            IClinicReviewRepository reviewRepository,
            IDoctorRepository doctorRepository,
            IClinicFacilityRepository facilityRepository,
            IClinicServiceRepository clinicServiceRepository,
            ILogger<ClinicService> logger)
        {
            _clinicRepository = clinicRepository ?? throw new ArgumentNullException(nameof(clinicRepository));
            _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
            _doctorRepository = doctorRepository ?? throw new ArgumentNullException(nameof(doctorRepository));
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _clinicServiceRepository = clinicServiceRepository ?? throw new ArgumentNullException(nameof(clinicServiceRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<ClinicDto>> GetClinicAsync(int clinicId)
        {
            _logger.LogInformation("Fetching clinic by ID: {ClinicId}", clinicId);

            var result = await _clinicRepository.GetClinicAsync(clinicId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch clinic {ClinicId}: {Message}", clinicId, result.Message);
                return OperationResult<ClinicDto>.Failure(result.Message);
            }

            _logger.LogInformation("Clinic fetched successfully - ClinicId: {ClinicId}", result.Data?.ClinicId);
            return OperationResult<ClinicDto>.Success(result.Data!, result.Message);
        }
        public async Task<OperationResult<ClinicDetailsDto>> GetClinicDetailsAsync(int clinicId)
        {
            _logger.LogInformation("Fetching clinic details by ID: {ClinicId}", clinicId);

            var clinic = await _clinicRepository.GetClinicAsync(clinicId);
            if (!clinic.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch clinic details {ClinicId}: {Message}", clinicId, clinic.Message);
                return OperationResult<ClinicDetailsDto>.Failure(clinic.Message);
            }
            var services = await _clinicServiceRepository.GetServicesAsync(new hospital_booking.Data.DTOs.ClinicService.ClinicServicesRequestDto { ClinicId = clinicId, Limit = 100 });
            var reviews = await _reviewRepository.GetReviewsAsync(new hospital_booking.Data.DTOs.ClinicReview.ClinicReviewsRequestDto { ClinicId = clinicId, Limit = 100 });
            var doctors = await _doctorRepository.GetDoctorsAsync(new hospital_booking.Data.DTOs.Doctor.DoctorsRequestDto { ClinicId = clinicId, Limit = 100 });
            var facilities = await _facilityRepository.GetFacilitiesByClinicAsync(clinicId);
            var clinicDetails = new ClinicDetailsDto
            {
                Clinic = clinic.Data!,
                Services = services.Data ?? new hospital_booking.Data.DTOs.ClinicService.ClinicServicesDto(),
                Reviews = reviews.Data ?? new hospital_booking.Data.DTOs.ClinicReview.ClinicReviewsDto(),
                Doctors = doctors.Data ?? new hospital_booking.Data.DTOs.Doctor.DoctorsDto(),
                Facilities = facilities.Data ?? new List<hospital_booking.Data.DTOs.ClinicFacility.ClinicFacilityDto>()
            };

            _logger.LogInformation("Clinic details fetched successfully - ClinicId: {ClinicId}", clinicDetails.Clinic.ClinicId);
            return OperationResult<ClinicDetailsDto>.Success(clinicDetails, clinic.Message);
        }

        public async Task<OperationResult<ClinicsDto>> GetClinicsAsync(ClinicsRequestDto requestDto)
        {
            _logger.LogInformation("Fetching clinics - Page: {Page}, Limit: {Limit}", requestDto.Page, requestDto.Limit);

            var result = await _clinicRepository.GetClinicsAsync(requestDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch clinics: {Message}", result.Message);
                return OperationResult<ClinicsDto>.Failure(result.Message);
            }

            return OperationResult<ClinicsDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<bool>> CreateClinicAsync(ClinicAddDto clinicDto)
        {
            _logger.LogInformation("Creating clinic: {Name}", clinicDto?.Name);

            var validationResult = await ClinicValidation.ValidateAddAsync(clinicDto!, _logger);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<bool>.Failure(validationResult.Message);
            }

            var result = await _clinicRepository.CreateClinicAsync(clinicDto!);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create clinic: {Message}", result.Message);
                return OperationResult<bool>.Failure(result.Message);
            }

            _logger.LogInformation("Clinic created successfully");
            return OperationResult<bool>.Success(true, result.Message);
        }

        public async Task<OperationResult<ClinicDto>> UpdateClinicAsync(int clinicId, ClinicUpdateDto clinicDto)
        {
            _logger.LogInformation("Updating clinic: {ClinicId}", clinicId);

            var validationResult = await ClinicValidation.ValidateUpdateAsync(clinicId, clinicDto, _clinicRepository, _logger);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<ClinicDto>.Failure(validationResult.Message);
            }

            var result = await _clinicRepository.UpdateClinicAsync(clinicId, clinicDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update clinic {ClinicId}: {Message}", clinicId, result.Message);
                return OperationResult<ClinicDto>.Failure(result.Message);
            }

            _logger.LogInformation("Clinic updated successfully - ClinicId: {ClinicId}", result.Data?.ClinicId);
            return OperationResult<ClinicDto>.Success(result.Data!, result.Message);
        }


        public async Task<OperationResult<bool>> DeleteClinicAsync(int clinicId)
        {
            _logger.LogInformation("Deleting clinic: {ClinicId}", clinicId);

            var result = await _clinicRepository.DeleteClinicAsync(clinicId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete clinic {ClinicId}: {Message}", clinicId, result.Message);
                return OperationResult<bool>.Failure(result.Message);
            }

            _logger.LogInformation("Clinic deleted successfully - ClinicId: {ClinicId}", clinicId);
            return OperationResult<bool>.Success(result.Data, result.Message);
        }
    }
}
