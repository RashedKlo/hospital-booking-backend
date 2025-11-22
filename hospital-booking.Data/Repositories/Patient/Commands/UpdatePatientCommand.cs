using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Patient.Helpers;

namespace hospital_booking.Data.Repositories.Patient.Commands
{
    public static class UpdatePatientCommand
    {
        private const string UpdateSql = @"
UPDATE dbo.patients
SET user_id = @UserId,
    full_name = @FullName,
    birthDate = @BirthDate,
    gender = @Gender,
    notes = @Notes
WHERE patient_id = @PatientId;

SELECT patient_id, user_id, full_name, birthDate, gender, notes
FROM dbo.patients
WHERE patient_id = @PatientId;
";

        public static async Task<OperationResult<PatientDto>> ExecuteAsync(int patientId, PatientDto dto, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("UpdatePatientCommand received null dto");
                return OperationResult<PatientDto>.Failure("Patient data is required");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(UpdateSql, connection);
                command.Parameters.AddWithValue("@PatientId", patientId);
                command.Parameters.AddWithValue("@UserId", (object?)dto.UserId ?? DBNull.Value);
                command.Parameters.AddWithValue("@FullName", dto.FullName ?? string.Empty);
                command.Parameters.AddWithValue("@BirthDate", (object?)dto.BirthDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@Gender", dto.Gender ?? string.Empty);
                command.Parameters.AddWithValue("@Notes", dto.Notes ?? string.Empty);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return OperationResult<PatientDto>.Failure("Patient not found");
                }

                var patient = PatientMapper.MapFromReader(reader);
                return OperationResult<PatientDto>.Success(patient, "Patient updated successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating patient: {Error}", ex.Message);
                return OperationResult<PatientDto>.Failure("Database operation failed");
            }
        }
    }
}
