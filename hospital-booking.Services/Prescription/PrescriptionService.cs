using System;
using System.Collections.Generic;
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
        private readonly ILogger<PrescriptionService> _logger;

        public PrescriptionService(IPrescriptionRepository prescriptionRepository, ILogger<PrescriptionService> logger)
        {
            _prescriptionRepository = prescriptionRepository ?? throw new ArgumentNullException(nameof(prescriptionRepository));
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

        public async Task<OperationResult<List<PrescriptionDto>>> GetPrescriptionsAsync(int page, int limit)
        {
            _logger.LogInformation("Fetching prescriptions - Page: {Page}, Limit: {Limit}", page, limit);

            var result = await _prescriptionRepository.GetPrescriptionsAsync(page, limit);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch prescriptions: {Message}", result.Message);
                return OperationResult<List<PrescriptionDto>>.Failure(result.Message);
            }

            _logger.LogInformation("Fetched {Count} prescriptions successfully", result.Data?.Count ?? 0);
            return OperationResult<List<PrescriptionDto>>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<List<PrescriptionDto>>> GetPrescriptionsByAppointmentAsync(int appointmentId)
        {
            _logger.LogInformation("Fetching prescriptions by AppointmentId: {AppointmentId}", appointmentId);

            var result = await _prescriptionRepository.GetPrescriptionsByAppointmentAsync(appointmentId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch prescriptions for appointment {AppointmentId}: {Message}", appointmentId, result.Message);
                return OperationResult<List<PrescriptionDto>>.Failure(result.Message);
            }

            _logger.LogInformation("Fetched {Count} prescriptions for AppointmentId: {AppointmentId}", result.Data?.Count ?? 0, appointmentId);
            return OperationResult<List<PrescriptionDto>>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<PrescriptionDto>> CreatePrescriptionAsync(PrescriptionDto prescriptionDto)
        {
            if (prescriptionDto == null)
            {
                _logger.LogWarning("Create prescription attempted with null data");
                return OperationResult<PrescriptionDto>.Failure("Prescription data is required");
            }

            _logger.LogInformation("Creating prescription for AppointmentId: {AppointmentId}", prescriptionDto.AppointmentId);

            var result = await _prescriptionRepository.CreatePrescriptionAsync(prescriptionDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create prescription: {Message}", result.Message);
                return OperationResult<PrescriptionDto>.Failure(result.Message);
            }

            _logger.LogInformation("Prescription created successfully - PrescriptionId: {PrescriptionId}", result.Data?.PrescriptionId);
            return OperationResult<PrescriptionDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<PrescriptionDto>> UpdatePrescriptionAsync(int prescriptionId, PrescriptionDto prescriptionDto)
        {
            if (prescriptionDto == null)
            {
                _logger.LogWarning("Update prescription attempted with null data");
                return OperationResult<PrescriptionDto>.Failure("Prescription data is required");
            }

            _logger.LogInformation("Updating prescription: {PrescriptionId}", prescriptionId);

            var result = await _prescriptionRepository.UpdatePrescriptionAsync(prescriptionId, prescriptionDto);

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
