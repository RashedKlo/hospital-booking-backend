using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.DoctorSchedule.Commands
{
    public static class DeleteDoctorScheduleCommand
    {
        private const string DeleteScheduleSql = @"
            DELETE FROM doctor_schedules 
            WHERE id = @ScheduleId";

        public static async Task<OperationResult<bool>> ExecuteAsync(
            int scheduleId,
            ILogger logger,
            string connectionString)
        {
            logger.LogInformation("Deleting schedule: {Id}", scheduleId);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(DeleteScheduleSql, connection);
                command.Parameters.AddWithValue("@ScheduleId", scheduleId);

                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    logger.LogWarning("Schedule not found for deletion: {Id}", scheduleId);
                    return OperationResult<bool>.Failure("Schedule not found");
                }

                logger.LogInformation("Schedule deleted successfully: {Id}", scheduleId);
                return OperationResult<bool>.Success(true, "Schedule deleted successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting schedule: {Id}", scheduleId);
                return OperationResult<bool>.Failure("Schedule deletion failed");
            }
        }
    }
}