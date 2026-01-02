using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Appointment;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Appointment.Commands
{
    public static class CreateAppointmentCommand
    {
        private const string CreateSql = @"
INSERT INTO dbo.appointments (patient_id, doctor_id, appointment_time, reason, status)
VALUES (@PatientId, @DoctorId, @AppointmentTime, @Reason, @Status);
";

        public static async Task<OperationResult<bool>> ExecuteAsync(AppointmentAddDto dto, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("CreateAppointmentCommand received null dto");
                return OperationResult<bool>.Failure("Appointment data is required");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(CreateSql, connection);
                command.Parameters.AddWithValue("@PatientId", dto.PatientId);
                command.Parameters.AddWithValue("@DoctorId", dto.DoctorId);
                command.Parameters.AddWithValue("@AppointmentTime", dto.AppointmentTime);
                command.Parameters.AddWithValue("@Reason", (object)dto.Reason ?? DBNull.Value);
                command.Parameters.AddWithValue("@Status", (object)dto.Status ?? "pending");

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    return OperationResult<bool>.Success(true, "Appointment created successfully");
                }
                return OperationResult<bool>.Failure("Failed to create appointment");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating appointment: {Error}", ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
        }
    }
}
