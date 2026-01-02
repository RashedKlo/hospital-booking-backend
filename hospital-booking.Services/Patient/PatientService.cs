using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using hospital_booking.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.Patient
{
    public sealed class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<PatientService> _logger;

        public PatientService(IPatientRepository patientRepository, ILogger<PatientService> logger)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<PatientDto>> GetPatientAsync(int patientId)
        {
            _logger.LogInformation("Fetching patient by ID: {PatientId}", patientId);

            var result = await _patientRepository.GetPatientAsync(patientId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch patient {PatientId}: {Message}", patientId, result.Message);
                return OperationResult<PatientDto>.Failure(result.Message);
            }

            _logger.LogInformation("Patient fetched successfully - PatientId: {PatientId}", result.Data?.PatientId);
            return OperationResult<PatientDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<PatientsDto>> GetPatientsAsync(PatientsRequestDto requestDto)
        {
            _logger.LogInformation("Fetching patients - Page: {Page}, Limit: {Limit}", requestDto.Page, requestDto.Limit);

            var result = await _patientRepository.GetPatientsAsync(requestDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch patients: {Message}", result.Message);
                return OperationResult<PatientsDto>.Failure(result.Message);
            }

            return OperationResult<PatientsDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<bool>> CreatePatientAsync(PatientAddDto dto)
        {
            _logger.LogInformation("Creating patient: {FullName}", dto?.FullName);

            var validationResult = await PatientValidation.ValidateAddAsync(dto!, _logger);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<bool>.Failure(validationResult.Message);
            }

            var result = await _patientRepository.CreatePatientAsync(dto!);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create patient: {Message}", result.Message);
                return OperationResult<bool>.Failure(result.Message);
            }

            _logger.LogInformation("Patient created successfully");
            return OperationResult<bool>.Success(true, result.Message);
        }

        public async Task<OperationResult<PatientDto>> UpdatePatientAsync(int patientId, PatientUpdateDto dto)
        {
            _logger.LogInformation("Updating patient: {PatientId}", patientId);

            var validationResult = await PatientValidation.ValidateUpdateAsync(patientId, dto, _patientRepository, _logger);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<PatientDto>.Failure(validationResult.Message);
            }

            var result = await _patientRepository.UpdatePatientAsync(patientId, dto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update patient {PatientId}: {Message}", patientId, result.Message);
                return OperationResult<PatientDto>.Failure(result.Message);
            }

            _logger.LogInformation("Patient updated successfully - PatientId: {PatientId}", result.Data?.PatientId);
            return OperationResult<PatientDto>.Success(result.Data!, result.Message);
        }


        public async Task<OperationResult<bool>> DeletePatientAsync(int patientId)
        {
            _logger.LogInformation("Deleting patient: {PatientId}", patientId);

            var result = await _patientRepository.DeletePatientAsync(patientId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete patient {PatientId}: {Message}", patientId, result.Message);
                return OperationResult<bool>.Failure(result.Message);
            }

            _logger.LogInformation("Patient deleted successfully - PatientId: {PatientId}", patientId);
            return OperationResult<bool>.Success(result.Data, result.Message);
        }
    }
}
