using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Appointment.Commands
{
    public static class DeleteAppointmentCommand
    {
        private const string DeleteSql = @"
DELETE FROM dbo.appointments
WHERE appointment_id = @AppointmentId;
";

        public static async Task<OperationResult<bool>> ExecuteAsync(int appointmentId, ILogger logger)
        {
            if (appointmentId <= 0)
            {
                logger.LogError("DeleteAppointmentCommand received invalid id: {AppointmentId}", appointmentId);
                return OperationResult<bool>.Failure("Invalid appointment id");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(DeleteSql, connection);
                command.Parameters.AddWithValue("@AppointmentId", appointmentId);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows == 0)
                {
                    return OperationResult<bool>.Failure("Appointment not found");
                }

                return OperationResult<bool>.Success(true, "Appointment deleted successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting appointment: {Error}", ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
        }
    }
}
