using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Patient.Queries;

namespace hospital_booking.Data.Repositories.Patient.Commands
{
    public static class UpdatePatientCommand
    {
        private const string UpdateSql = @"
UPDATE dbo.patients
SET full_name = ISNULL(@FullName, full_name),
    birthDate = ISNULL(@BirthDate, birthDate),
    gender = ISNULL(@Gender, gender),
    notes = ISNULL(@Notes, notes)
WHERE patient_id = @PatientId;
";

        public static async Task<OperationResult<PatientDto>> ExecuteAsync(int patientId, PatientUpdateDto dto, ILogger logger)
        {
            if (patientId <= 0) return OperationResult<PatientDto>.Failure("Invalid patient ID");
            if (dto == null) return OperationResult<PatientDto>.Failure("Data is required");

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(UpdateSql, connection);
                command.Parameters.AddWithValue("@PatientId", patientId);
                command.Parameters.AddWithValue("@FullName", (object?)dto.FullName ?? DBNull.Value);
                command.Parameters.AddWithValue("@BirthDate", (object?)dto.BirthDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@Gender", (object?)dto.Gender ?? DBNull.Value);
                command.Parameters.AddWithValue("@Notes", (object?)dto.Notes ?? DBNull.Value);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows == 0)
                {
                    return OperationResult<PatientDto>.Failure("Patient not found");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating patient: {Error}", ex.Message);
                return OperationResult<PatientDto>.Failure("Database operation failed");
            }

            return await GetPatientQuery.ExecuteAsync(patientId, logger);
        }
    }
}
