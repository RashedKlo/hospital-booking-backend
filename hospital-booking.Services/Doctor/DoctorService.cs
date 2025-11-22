using System;
using System.Collections.Generic;
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
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(IDoctorRepository doctorRepository, ILogger<DoctorService> logger)
        {
            _doctorRepository = doctorRepository ?? throw new ArgumentNullException(nameof(doctorRepository));
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

        public async Task<OperationResult<List<DoctorDto>>> GetDoctorsAsync(int page, int limit)
        {
            _logger.LogInformation("Fetching doctors - Page: {Page}, Limit: {Limit}", page, limit);

            var result = await _doctorRepository.GetDoctorsAsync(page, limit);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch doctors: {Message}", result.Message);
                return OperationResult<List<DoctorDto>>.Failure(result.Message);
            }

            _logger.LogInformation("Fetched {Count} doctors successfully", result.Data?.Count ?? 0);
            return OperationResult<List<DoctorDto>>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<DoctorDto>> CreateDoctorAsync(DoctorDto doctorDto)
        {
            if (doctorDto == null)
            {
                _logger.LogWarning("Create doctor attempted with null data");
                return OperationResult<DoctorDto>.Failure("Doctor data is required");
            }

            _logger.LogInformation("Creating doctor: {FullName}", doctorDto.FullName);

            var result = await _doctorRepository.CreateDoctorAsync(doctorDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create doctor: {Message}", result.Message);
                return OperationResult<DoctorDto>.Failure(result.Message);
            }

            _logger.LogInformation("Doctor created successfully - DoctorId: {DoctorId}", result.Data?.DoctorId);
            return OperationResult<DoctorDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<DoctorDto>> UpdateDoctorAsync(int doctorId, DoctorDto doctorDto)
        {
            if (doctorDto == null)
            {
                _logger.LogWarning("Update doctor attempted with null data");
                return OperationResult<DoctorDto>.Failure("Doctor data is required");
            }

            _logger.LogInformation("Updating doctor: {DoctorId}", doctorId);

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
