using System;
using System.Collections.Generic;
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
        private readonly ILogger<ClinicService> _logger;

        public ClinicService(IClinicRepository clinicRepository, ILogger<ClinicService> logger)
        {
            _clinicRepository = clinicRepository ?? throw new ArgumentNullException(nameof(clinicRepository));
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

        public async Task<OperationResult<List<ClinicDto>>> GetClinicsAsync(int page, int limit)
        {
            _logger.LogInformation("Fetching clinics - Page: {Page}, Limit: {Limit}", page, limit);

            var result = await _clinicRepository.GetClinicsAsync(page, limit);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch clinics: {Message}", result.Message);
                return OperationResult<List<ClinicDto>>.Failure(result.Message);
            }

            _logger.LogInformation("Fetched {Count} clinics successfully", result.Data?.Count ?? 0);
            return OperationResult<List<ClinicDto>>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<ClinicDto>> CreateClinicAsync(ClinicDto clinicDto)
        {
            if (clinicDto == null)
            {
                _logger.LogWarning("Create clinic attempted with null data");
                return OperationResult<ClinicDto>.Failure("Clinic data is required");
            }

            _logger.LogInformation("Creating clinic: {Title}", clinicDto.Title);

            var result = await _clinicRepository.CreateClinicAsync(clinicDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create clinic: {Message}", result.Message);
                return OperationResult<ClinicDto>.Failure(result.Message);
            }

            _logger.LogInformation("Clinic created successfully - ClinicId: {ClinicId}", result.Data?.ClinicId);
            return OperationResult<ClinicDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<ClinicDto>> UpdateClinicAsync(int clinicId, ClinicDto clinicDto)
        {
            if (clinicDto == null)
            {
                _logger.LogWarning("Update clinic attempted with null data");
                return OperationResult<ClinicDto>.Failure("Clinic data is required");
            }

            _logger.LogInformation("Updating clinic: {ClinicId}", clinicId);

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
