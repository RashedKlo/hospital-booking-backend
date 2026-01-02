using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Appointment;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Appointment.Queries;

namespace hospital_booking.Data.Repositories.Appointment.Commands
{
    public static class UpdateAppointmentCommand
    {
        private const string UpdateSql = @"
UPDATE dbo.appointments
SET appointment_time = ISNULL(@AppointmentTime, appointment_time),
    reason = ISNULL(@Reason, reason),
    status = ISNULL(@Status, status)
WHERE appointment_id = @AppointmentId;
";

        public static async Task<OperationResult<AppointmentDto>> ExecuteAsync(int appointmentId, AppointmentUpdateDto dto, ILogger logger)
        {
            if (appointmentId <= 0)
            {
                return OperationResult<AppointmentDto>.Failure("Invalid appointment id");
            }
            if (dto == null)
            {
                logger.LogError("UpdateAppointmentCommand received null dto");
                return OperationResult<AppointmentDto>.Failure("Appointment data is required");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(UpdateSql, connection);
                command.Parameters.AddWithValue("@AppointmentId", appointmentId);
                command.Parameters.AddWithValue("@AppointmentTime", (object?)dto.AppointmentTime ?? DBNull.Value);
                command.Parameters.AddWithValue("@Reason", (object?)dto.Reason ?? DBNull.Value);
                command.Parameters.AddWithValue("@Status", (object?)dto.Status ?? DBNull.Value);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows == 0)
                {
                    return OperationResult<AppointmentDto>.Failure("Appointment not found");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating appointment: {Error}", ex.Message);
                return OperationResult<AppointmentDto>.Failure("Database operation failed");
            }

            // Return full DTO with joins using the Query class
            return await GetAppointmentQuery.ExecuteAsync(appointmentId, logger);
        }
    }
}
