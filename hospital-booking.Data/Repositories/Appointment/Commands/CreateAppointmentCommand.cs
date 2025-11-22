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
    public static class CreateAppointmentCommand
    {
        private const string CreateSql = @"
INSERT INTO dbo.appointments (patient_id, doctor_id, appointment_time, reason, status)
OUTPUT inserted.appointment_id, inserted.patient_id, inserted.doctor_id, inserted.appointment_time, inserted.reason, inserted.status
VALUES (@PatientId, @DoctorId, @AppointmentTime, @Reason, @Status);
";

        public static async Task<OperationResult<AppointmentDto>> ExecuteAsync(AppointmentDto dto, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("CreateAppointmentCommand received null dto");
                return OperationResult<AppointmentDto>.Failure("Appointment data is required");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(CreateSql, connection);
                command.Parameters.AddWithValue("@PatientId", dto.PatientId);
                command.Parameters.AddWithValue("@DoctorId", dto.DoctorId);
                command.Parameters.AddWithValue("@AppointmentTime", dto.AppointmentTime);
                command.Parameters.AddWithValue("@Reason", dto.Reason ?? string.Empty);
                command.Parameters.AddWithValue("@Status", dto.Status ?? "pending");

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return OperationResult<AppointmentDto>.Failure("Appointment creation returned no result");
                }

                var appt = AppointmentMapper.MapFromReader(reader);
                return OperationResult<AppointmentDto>.Success(appt, "Appointment created successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating appointment: {Error}", ex.Message);
                return OperationResult<AppointmentDto>.Failure("Database operation failed");
            }
        }
    }
}
