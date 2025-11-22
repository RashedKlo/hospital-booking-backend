using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Appointment;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Appointment.Helpers;

namespace hospital_booking.Data.Repositories.Appointment.Queries
{
    public class GetAppointmentsQuery
    {
        private const string GetSql = @"
SELECT
    appointment_id,
    patient_id,
    doctor_id,
    appointment_time,
    reason,
    status
FROM dbo.appointments
ORDER BY appointment_id
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
";

        public static async Task<OperationResult<List<AppointmentDto>>> ExecuteAsync(int page, int limit, ILogger logger)
        {
            if (page <= 0 || limit <= 0)
            {
                logger.LogError("GetAppointmentsQuery received invalid pagination");
                return OperationResult<List<AppointmentDto>>.Failure("Invalid pagination");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetSql, connection);
                var offset = (page - 1) * limit;
                command.Parameters.AddWithValue("@Offset", offset);
                command.Parameters.AddWithValue("@Limit", limit);

                using var reader = await command.ExecuteReaderAsync();
                var list = new List<AppointmentDto>();
                while (await reader.ReadAsync())
                {
                    list.Add(AppointmentMapper.MapFromReader(reader));
                }

                return OperationResult<List<AppointmentDto>>.Success(list, "Appointments retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting appointments: {Error}", ex.Message);
                return OperationResult<List<AppointmentDto>>.Failure("Database operation failed");
            }
        }
    }
}
