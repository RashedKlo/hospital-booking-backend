using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Appointment;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using hospital_booking.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.Appointment
{
    public sealed class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(IAppointmentRepository appointmentRepository, ILogger<AppointmentService> logger)
        {
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<AppointmentDto>> GetAppointmentAsync(int appointmentId)
        {
            _logger.LogInformation("Fetching appointment by ID: {AppointmentId}", appointmentId);

            var result = await _appointmentRepository.GetAppointmentAsync(appointmentId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch appointment {AppointmentId}: {Message}", appointmentId, result.Message);
                return OperationResult<AppointmentDto>.Failure(result.Message);
            }

            _logger.LogInformation("Appointment fetched successfully - AppointmentId: {AppointmentId}", result.Data?.AppointmentId);
            return OperationResult<AppointmentDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<List<AppointmentDto>>> GetAppointmentsAsync(int page, int limit)
        {
            _logger.LogInformation("Fetching appointments - Page: {Page}, Limit: {Limit}", page, limit);

            var result = await _appointmentRepository.GetAppointmentsAsync(page, limit);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch appointments: {Message}", result.Message);
                return OperationResult<List<AppointmentDto>>.Failure(result.Message);
            }

            _logger.LogInformation("Fetched {Count} appointments successfully", result.Data?.Count ?? 0);
            return OperationResult<List<AppointmentDto>>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<AppointmentDto>> CreateAppointmentAsync(AppointmentDto appointmentDto)
        {
            if (appointmentDto == null)
            {
                _logger.LogWarning("Create appointment attempted with null data");
                return OperationResult<AppointmentDto>.Failure("Appointment data is required");
            }

            _logger.LogInformation("Creating appointment for PatientId: {PatientId}, DoctorId: {DoctorId}", 
                appointmentDto.PatientId, appointmentDto.DoctorId);

            var result = await _appointmentRepository.CreateAppointmentAsync(appointmentDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create appointment: {Message}", result.Message);
                return OperationResult<AppointmentDto>.Failure(result.Message);
            }

            _logger.LogInformation("Appointment created successfully - AppointmentId: {AppointmentId}", result.Data?.AppointmentId);
            return OperationResult<AppointmentDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<AppointmentDto>> UpdateAppointmentAsync(int appointmentId, AppointmentDto appointmentDto)
        {
            if (appointmentDto == null)
            {
                _logger.LogWarning("Update appointment attempted with null data");
                return OperationResult<AppointmentDto>.Failure("Appointment data is required");
            }

            _logger.LogInformation("Updating appointment: {AppointmentId}", appointmentId);

            var result = await _appointmentRepository.UpdateAppointmentAsync(appointmentId, appointmentDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update appointment {AppointmentId}: {Message}", appointmentId, result.Message);
                return OperationResult<AppointmentDto>.Failure(result.Message);
            }

            _logger.LogInformation("Appointment updated successfully - AppointmentId: {AppointmentId}", result.Data?.AppointmentId);
            return OperationResult<AppointmentDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<bool>> DeleteAppointmentAsync(int appointmentId)
        {
            _logger.LogInformation("Deleting appointment: {AppointmentId}", appointmentId);

            var result = await _appointmentRepository.DeleteAppointmentAsync(appointmentId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete appointment {AppointmentId}: {Message}", appointmentId, result.Message);
                return OperationResult<bool>.Failure(result.Message);
            }

            _logger.LogInformation("Appointment deleted successfully - AppointmentId: {AppointmentId}", appointmentId);
            return OperationResult<bool>.Success(result.Data, result.Message);
        }
    }
}
