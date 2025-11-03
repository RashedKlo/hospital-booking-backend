using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.DoctorSchedule;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Repositories.DoctorSchedule.Commands;
using hospital_booking.Data.Repositories.DoctorSchedule.Queries;
using hospital_booking.Data.Results;
using hospital_booking.Data.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace hospital_booking.Data.Repositories.DoctorSchedule
{
    public class DoctorScheduleRepository : IDoctorScheduleRepository
    {
        private readonly ILogger<DoctorScheduleRepository> _logger;
        private readonly string _connectionString;

        public DoctorScheduleRepository(
            ILogger<DoctorScheduleRepository> logger,
            IOptions<DatabaseSettings> databaseSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionString = databaseSettings?.Value?.ConnectionString 
                ?? throw new ArgumentNullException(nameof(databaseSettings));
        }

        public async Task<OperationResult<DoctorScheduleDto>> CreateScheduleAsync(CreateDoctorScheduleDto dto)
        {
            return await CreateDoctorScheduleCommand.ExecuteAsync(dto, _logger, _connectionString);
        }

        public async Task<OperationResult<DoctorScheduleDto>> UpdateScheduleAsync(int scheduleId, UpdateDoctorScheduleDto dto)
        {
            return await UpdateDoctorScheduleCommand.ExecuteAsync(scheduleId, dto, _logger, _connectionString);
        }

        public async Task<OperationResult<bool>> DeleteScheduleAsync(int scheduleId)
        {
            return await DeleteDoctorScheduleCommand.ExecuteAsync(scheduleId, _logger, _connectionString);
        }

        public async Task<OperationResult<DoctorScheduleDto>> GetScheduleByIdAsync(int scheduleId)
        {
            return await GetScheduleByIdQuery.ExecuteAsync(scheduleId, _logger, _connectionString);
        }

        public async Task<OperationResult<List<DoctorScheduleDto>>> GetSchedulesByDoctorAsync(int doctorId)
        {
            return await GetSchedulesByDoctorQuery.ExecuteAsync(doctorId, _logger, _connectionString);
        }

        public async Task<OperationResult<List<DoctorScheduleDto>>> GetAllSchedulesAsync()
        {
            return await GetAllSchedulesQuery.ExecuteAsync(_logger, _connectionString);
        }
    }
}