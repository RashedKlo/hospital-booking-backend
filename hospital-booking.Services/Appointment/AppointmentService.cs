using System;
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
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(
            IAppointmentRepository appointmentRepository, 
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository,
            ILogger<AppointmentService> logger)
        {
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _doctorRepository = doctorRepository ?? throw new ArgumentNullException(nameof(doctorRepository));
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

        public async Task<OperationResult<AppointmentsDto>> GetAppointmentsAsync(AppointmentsRequestDto requestDto)
        {
            _logger.LogInformation("Fetching appointments - Page: {Page}, Limit: {Limit}", requestDto.Page, requestDto.Limit);

            var result = await _appointmentRepository.GetAppointmentsAsync(requestDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch appointments: {Message}", result.Message);
                return OperationResult<AppointmentsDto>.Failure(result.Message);
            }

            return OperationResult<AppointmentsDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<bool>> CreateAppointmentAsync(AppointmentAddDto appointmentDto)
        {
            _logger.LogInformation("Creating appointment for PatientId: {PatientId}, DoctorId: {DoctorId}", 
                appointmentDto?.PatientId, appointmentDto?.DoctorId);

            var validationResult = await AppointmentValidation.ValidateAddAsync(appointmentDto!, _patientRepository, _doctorRepository, _logger);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<bool>.Failure(validationResult.Message);
            }

            var result = await _appointmentRepository.CreateAppointmentAsync(appointmentDto!);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create appointment: {Message}", result.Message);
                return OperationResult<bool>.Failure(result.Message);
            }

            _logger.LogInformation("Appointment created successfully");
            return OperationResult<bool>.Success(true, result.Message);
        }

        public async Task<OperationResult<AppointmentDto>> UpdateAppointmentAsync(int appointmentId, AppointmentUpdateDto appointmentDto)
        {
            _logger.LogInformation("Updating appointment: {AppointmentId}", appointmentId);

            var validationResult = await AppointmentValidation.ValidateUpdateAsync(appointmentId, appointmentDto, _appointmentRepository, _logger);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<AppointmentDto>.Failure(validationResult.Message);
            }

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
