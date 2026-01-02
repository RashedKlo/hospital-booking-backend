using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Appointment;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Repositories.Appointment.Commands;
using hospital_booking.Data.Repositories.Appointment.Queries;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Data.Repositories.Appointment
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ILogger<AppointmentRepository> _logger;

        public AppointmentRepository(ILogger<AppointmentRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<bool>> CreateAppointmentAsync(AppointmentAddDto dto)
        {
            return await CreateAppointmentCommand.ExecuteAsync(dto, _logger);
        }

        public async Task<OperationResult<AppointmentDto>> UpdateAppointmentAsync(int appointmentId, AppointmentUpdateDto dto)
        {
            return await UpdateAppointmentCommand.ExecuteAsync(appointmentId, dto, _logger);
        }

        public async Task<OperationResult<bool>> DeleteAppointmentAsync(int appointmentId)
        {
            return await DeleteAppointmentCommand.ExecuteAsync(appointmentId, _logger);
        }

        public async Task<OperationResult<AppointmentDto>> GetAppointmentAsync(int appointmentId)
        {
            return await GetAppointmentQuery.ExecuteAsync(appointmentId, _logger);
        }

        public async Task<OperationResult<AppointmentsDto>> GetAppointmentsAsync(AppointmentsRequestDto requestDto)
        {
            return await GetAppointmentsQuery.ExecuteAsync(requestDto, _logger);
        }
    }
}
