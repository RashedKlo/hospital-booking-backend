using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Prescription;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using hospital_booking.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.Prescription
{
    public sealed class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger<PrescriptionService> _logger;

        public PrescriptionService(
            IPrescriptionRepository prescriptionRepository, 
            IAppointmentRepository appointmentRepository,
            ILogger<PrescriptionService> logger)
        {
            _prescriptionRepository = prescriptionRepository ?? throw new ArgumentNullException(nameof(prescriptionRepository));
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<PrescriptionDto>> GetPrescriptionAsync(int prescriptionId)
        {
            _logger.LogInformation("Fetching prescription by ID: {PrescriptionId}", prescriptionId);

            var result = await _prescriptionRepository.GetPrescriptionAsync(prescriptionId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch prescription {PrescriptionId}: {Message}", prescriptionId, result.Message);
                return OperationResult<PrescriptionDto>.Failure(result.Message);
            }

            _logger.LogInformation("Prescription fetched successfully - PrescriptionId: {PrescriptionId}", result.Data?.PrescriptionId);
            return OperationResult<PrescriptionDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<PrescriptionsDto>> GetPrescriptionsAsync(PrescriptionsRequestDto requestDto)
        {
            _logger.LogInformation("Fetching prescriptions - Page: {Page}, Limit: {Limit}", requestDto.Page, requestDto.Limit);

            var result = await _prescriptionRepository.GetPrescriptionsAsync(requestDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch prescriptions: {Message}", result.Message);
                return OperationResult<PrescriptionsDto>.Failure(result.Message);
            }

            return OperationResult<PrescriptionsDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<bool>> CreatePrescriptionAsync(PrescriptionAddDto dto)
        {
            _logger.LogInformation("Creating prescription for AppointmentId: {AppointmentId}", dto?.AppointmentId);

            var validationResult = await PrescriptionValidation.ValidateAddAsync(dto!, _appointmentRepository, _logger);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<bool>.Failure(validationResult.Message);
            }

            var result = await _prescriptionRepository.CreatePrescriptionAsync(dto!);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create prescription: {Message}", result.Message);
                return OperationResult<bool>.Failure(result.Message);
            }

            _logger.LogInformation("Prescription created successfully");
            return OperationResult<bool>.Success(true, result.Message);
        }

        public async Task<OperationResult<PrescriptionDto>> UpdatePrescriptionAsync(int prescriptionId, PrescriptionUpdateDto dto)
        {
            _logger.LogInformation("Updating prescription: {PrescriptionId}", prescriptionId);

            var validationResult = await PrescriptionValidation.ValidateUpdateAsync(prescriptionId, dto, _prescriptionRepository, _logger);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<PrescriptionDto>.Failure(validationResult.Message);
            }

            var result = await _prescriptionRepository.UpdatePrescriptionAsync(prescriptionId, dto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update prescription {PrescriptionId}: {Message}", prescriptionId, result.Message);
                return OperationResult<PrescriptionDto>.Failure(result.Message);
            }

            _logger.LogInformation("Prescription updated successfully - PrescriptionId: {PrescriptionId}", result.Data?.PrescriptionId);
            return OperationResult<PrescriptionDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<bool>> DeletePrescriptionAsync(int prescriptionId)
        {
            _logger.LogInformation("Deleting prescription: {PrescriptionId}", prescriptionId);

            var result = await _prescriptionRepository.DeletePrescriptionAsync(prescriptionId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete prescription {PrescriptionId}: {Message}", prescriptionId, result.Message);
                return OperationResult<bool>.Failure(result.Message);
            }

            _logger.LogInformation("Prescription deleted successfully - PrescriptionId: {PrescriptionId}", prescriptionId);
            return OperationResult<bool>.Success(result.Data, result.Message);
        }

    }
}
