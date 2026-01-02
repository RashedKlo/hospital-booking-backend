using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Appointment;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Appointment.Helpers;

namespace hospital_booking.Data.Repositories.Appointment.Queries
{
    public class GetAppointmentQuery
    {
        private const string GetSql = @"
SELECT 
    a.appointment_id, a.patient_id, a.doctor_id, a.appointment_time, a.reason, a.status,
    p.patient_id, p.full_name, p.birthDate, p.gender, p.notes,
    d.doctor_id, d.clinic_id, d.full_name, d.bio, d.phone, d.is_active, d.experience_years
FROM dbo.appointments a
INNER JOIN dbo.patients p ON a.patient_id = p.patient_id
INNER JOIN dbo.doctors d ON a.doctor_id = d.doctor_id
WHERE a.appointment_id = @AppointmentId;
";

        public static async Task<OperationResult<AppointmentDto>> ExecuteAsync(int appointmentId, ILogger logger)
        {
            if (appointmentId <= 0)
            {
                logger.LogError("GetAppointmentQuery received invalid id: {Id}", appointmentId);
                return OperationResult<AppointmentDto>.Failure("Invalid appointment id");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetSql, connection);
                command.Parameters.AddWithValue("@AppointmentId", appointmentId);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return OperationResult<AppointmentDto>.Failure("Appointment not found");
                }

                var dto = AppointmentMapper.MapFromReader(reader);
                return OperationResult<AppointmentDto>.Success(dto, "Appointment retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting appointment: {Error}", ex.Message);
                return OperationResult<AppointmentDto>.Failure("Database operation failed");
            }
        }
    }
}
