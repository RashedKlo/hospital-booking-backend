using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.DoctorSchedule;
using hospital_booking.Data.Repositories.DoctorSchedule.Helpers;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.DoctorSchedule.Queries
{
    public static class GetScheduleByIdQuery
    {
        private const string GetScheduleSql = @"
            SELECT * FROM doctor_schedules 
            WHERE id = @ScheduleId";

        public static async Task<OperationResult<DoctorScheduleDto>> ExecuteAsync(
            int scheduleId,
            ILogger logger,
            string connectionString)
        {
            logger.LogDebug("Getting schedule by ID: {Id}", scheduleId);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetScheduleSql, connection);
                command.Parameters.AddWithValue("@ScheduleId", scheduleId);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogDebug("Schedule not found: {Id}", scheduleId);
                    return OperationResult<DoctorScheduleDto>.Failure("Schedule not found");
                }

                var schedule = DoctorScheduleMapper.MapDoctorScheduleFromReader(reader);
                var scheduleDto = DoctorScheduleMapper.MapToDto(schedule);

                return OperationResult<DoctorScheduleDto>.Success(scheduleDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting schedule by ID: {Id}", scheduleId);
                return OperationResult<DoctorScheduleDto>.Failure("Failed to retrieve schedule");
            }
        }
    }
}