using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.DoctorSchedule;
using hospital_booking.Data.Repositories.DoctorSchedule.Helpers;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.DoctorSchedule.Queries
{
    public static class GetAllSchedulesQuery
    {
        private const string GetAllSchedulesSql = @"
            SELECT * FROM doctor_schedules 
            ORDER BY doctor_id, 
                CASE day_of_week
                    WHEN 'sunday' THEN 1
                    WHEN 'monday' THEN 2
                    WHEN 'tuesday' THEN 3
                    WHEN 'wednesday' THEN 4
                    WHEN 'thursday' THEN 5
                    WHEN 'friday' THEN 6
                    WHEN 'saturday' THEN 7
                END, start_time";

        public static async Task<OperationResult<List<DoctorScheduleDto>>> ExecuteAsync(
            ILogger logger,
            string connectionString)
        {
            logger.LogDebug("Getting all schedules");

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetAllSchedulesSql, connection);
                var schedules = new List<DoctorScheduleDto>();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var schedule = DoctorScheduleMapper.MapDoctorScheduleFromReader(reader);
                    schedules.Add(DoctorScheduleMapper.MapToDto(schedule));
                }

                logger.LogDebug("Retrieved {Count} schedules", schedules.Count);
                return OperationResult<List<DoctorScheduleDto>>.Success(schedules);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting all schedules");
                return OperationResult<List<DoctorScheduleDto>>.Failure("Failed to retrieve schedules");
            }
        }
    }
}