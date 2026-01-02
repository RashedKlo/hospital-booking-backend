using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using hospital_booking.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.Doctor
{
    public sealed class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(
            IDoctorRepository doctorRepository, 
            IClinicRepository clinicRepository,
            ILogger<DoctorService> logger)
        {
            _doctorRepository = doctorRepository ?? throw new ArgumentNullException(nameof(doctorRepository));
            _clinicRepository = clinicRepository ?? throw new ArgumentNullException(nameof(clinicRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<DoctorDto>> GetDoctorAsync(int doctorId)
        {
            _logger.LogInformation("Fetching doctor by ID: {DoctorId}", doctorId);

            var result = await _doctorRepository.GetDoctorAsync(doctorId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch doctor {DoctorId}: {Message}", doctorId, result.Message);
                return OperationResult<DoctorDto>.Failure(result.Message);
            }

            _logger.LogInformation("Doctor fetched successfully - DoctorId: {DoctorId}", result.Data?.DoctorId);
            return OperationResult<DoctorDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<DoctorsDto>> GetDoctorsAsync(DoctorsRequestDto requestDto)
        {
            _logger.LogInformation("Fetching doctors - Page: {Page}, Limit: {Limit}", requestDto.Page, requestDto.Limit);

            var result = await _doctorRepository.GetDoctorsAsync(requestDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch doctors on page {Page}: {Message}", requestDto.Page, result.Message);
                return OperationResult<DoctorsDto>.Failure(result.Message);
            }


            return OperationResult<DoctorsDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<bool>> CreateDoctorAsync(DoctorAddDto doctorDto)
        {
            _logger.LogInformation("Creating doctor for ClinicId: {ClinicId}, Name: {Name}", doctorDto?.ClinicId, doctorDto?.FullName);

            var validationResult = await DoctorValidation.ValidateAddAsync(doctorDto!, _clinicRepository, _logger);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<bool>.Failure(validationResult.Message);
            }

            var result = await _doctorRepository.CreateDoctorAsync(doctorDto!);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create doctor: {Message}", result.Message);
                return OperationResult<bool>.Failure(result.Message);
            }

            _logger.LogInformation("Doctor created successfully");
            return OperationResult<bool>.Success(true, result.Message);
        }

        public async Task<OperationResult<DoctorDto>> UpdateDoctorAsync(int doctorId, DoctorUpdateDto doctorDto)
        {
            _logger.LogInformation("Updating doctor: {DoctorId}", doctorId);

            var validationResult = await DoctorValidation.ValidateUpdateAsync(doctorId, doctorDto, _doctorRepository, _logger);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<DoctorDto>.Failure(validationResult.Message);
            }

            var result = await _doctorRepository.UpdateDoctorAsync(doctorId, doctorDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update doctor {DoctorId}: {Message}", doctorId, result.Message);
                return OperationResult<DoctorDto>.Failure(result.Message);
            }

            _logger.LogInformation("Doctor updated successfully - DoctorId: {DoctorId}", result.Data?.DoctorId);
            return OperationResult<DoctorDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<bool>> DeleteDoctorAsync(int doctorId)
        {
            _logger.LogInformation("Deleting doctor: {DoctorId}", doctorId);

            var result = await _doctorRepository.DeleteDoctorAsync(doctorId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete doctor {DoctorId}: {Message}", doctorId, result.Message);
                return OperationResult<bool>.Failure(result.Message);
            }

            _logger.LogInformation("Doctor deleted successfully - DoctorId: {DoctorId}", doctorId);
            return OperationResult<bool>.Success(result.Data, result.Message);
        }

    }
}
