using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Appointment;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Appointment.Helpers;

namespace hospital_booking.Data.Repositories.Appointment.Commands
{
    public static class UpdateAppointmentCommand
    {
        private const string UpdateSql = @"
UPDATE dbo.appointments
SET patient_id = @PatientId,
    doctor_id = @DoctorId,
    appointment_time = @AppointmentTime,
    reason = @Reason,
    status = @Status
WHERE appointment_id = @AppointmentId;

SELECT appointment_id, patient_id, doctor_id, appointment_time, reason, status
FROM dbo.appointments
WHERE appointment_id = @AppointmentId;
";

        public static async Task<OperationResult<AppointmentDto>> ExecuteAsync(int appointmentId, AppointmentDto dto, ILogger logger)
        {
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
                command.Parameters.AddWithValue("@PatientId", dto.PatientId);
                command.Parameters.AddWithValue("@DoctorId", dto.DoctorId);
                command.Parameters.AddWithValue("@AppointmentTime", dto.AppointmentTime);
                command.Parameters.AddWithValue("@Reason", dto.Reason ?? string.Empty);
                command.Parameters.AddWithValue("@Status", dto.Status ?? string.Empty);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return OperationResult<AppointmentDto>.Failure("Appointment not found");
                }

                var appt = AppointmentMapper.MapFromReader(reader);
                return OperationResult<AppointmentDto>.Success(appt, "Appointment updated successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating appointment: {Error}", ex.Message);
                return OperationResult<AppointmentDto>.Failure("Database operation failed");
            }
        }
    }
}
