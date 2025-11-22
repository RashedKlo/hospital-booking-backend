using System;
using System.Collections.Generic;
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

        public async Task<OperationResult<AppointmentDto>> CreateAppointmentAsync(AppointmentDto dto)
        {
            return await CreateAppointmentCommand.ExecuteAsync(dto, _logger);
        }

        public async Task<OperationResult<AppointmentDto>> UpdateAppointmentAsync(int appointmentId, AppointmentDto dto)
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

        public async Task<OperationResult<List<AppointmentDto>>> GetAppointmentsAsync(int page, int limit)
        {
            return await GetAppointmentsQuery.ExecuteAsync(page, limit, _logger);
        }
    }
}
