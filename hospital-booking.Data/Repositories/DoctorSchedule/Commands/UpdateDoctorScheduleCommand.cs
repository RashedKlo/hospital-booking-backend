using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.DoctorSchedule;
using hospital_booking.Data.Repositories.DoctorSchedule.Helpers;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.DoctorSchedule.Commands
{
    public static class UpdateDoctorScheduleCommand
    {
        private const string UpdateScheduleSql = @"
            UPDATE doctor_schedules 
            SET start_time = @StartTime,
                end_time = @EndTime,
                is_available = @IsAvailable,
                blocked_reason = @BlockedReason,
                updated_at = GETDATE()
            OUTPUT INSERTED.*
            WHERE id = @ScheduleId";

        public static async Task<OperationResult<DoctorScheduleDto>> ExecuteAsync(
            int scheduleId,
            UpdateDoctorScheduleDto dto,
            ILogger logger,
            string connectionString)
        {
            logger.LogInformation("Updating schedule: {Id}", scheduleId);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(UpdateScheduleSql, connection);
                command.Parameters.AddWithValue("@ScheduleId", scheduleId);
                command.Parameters.AddWithValue("@StartTime", dto.StartTime);
                command.Parameters.AddWithValue("@EndTime", dto.EndTime);
                command.Parameters.AddWithValue("@IsAvailable", dto.IsAvailable);
                command.Parameters.AddWithValue("@BlockedReason", (object?)dto.BlockedReason ?? DBNull.Value);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogWarning("Schedule not found for update: {Id}", scheduleId);
                    return OperationResult<DoctorScheduleDto>.Failure("Schedule not found");
                }

                var schedule = DoctorScheduleMapper.MapDoctorScheduleFromReader(reader);
                var scheduleDto = DoctorScheduleMapper.MapToDto(schedule);

                logger.LogInformation("Schedule updated successfully: {Id}", scheduleId);
                return OperationResult<DoctorScheduleDto>.Success(scheduleDto, "Schedule updated successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating schedule: {Id}", scheduleId);
                return OperationResult<DoctorScheduleDto>.Failure("Schedule update failed");
            }
        }
    }
}