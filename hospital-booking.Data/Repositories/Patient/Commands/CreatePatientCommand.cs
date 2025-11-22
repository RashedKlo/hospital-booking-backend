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
    public static class CreatePatientCommand
    {
        private const string CreateSql = @"
INSERT INTO dbo.patients (user_id, full_name, birthDate, gender, notes)
OUTPUT inserted.patient_id, inserted.user_id, inserted.full_name, inserted.birthDate, inserted.gender, inserted.notes
VALUES (@UserId, @FullName, @BirthDate, @Gender, @Notes);
";

        public static async Task<OperationResult<PatientDto>> ExecuteAsync(PatientDto dto, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("CreatePatientCommand received null dto");
                return OperationResult<PatientDto>.Failure("Patient data is required");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(CreateSql, connection);
                command.Parameters.AddWithValue("@UserId", (object?)dto.UserId ?? DBNull.Value);
                command.Parameters.AddWithValue("@FullName", dto.FullName ?? string.Empty);
                command.Parameters.AddWithValue("@BirthDate", (object?)dto.BirthDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@Gender", dto.Gender ?? string.Empty);
                command.Parameters.AddWithValue("@Notes", dto.Notes ?? string.Empty);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return OperationResult<PatientDto>.Failure("Patient creation returned no result");
                }

                var patient = PatientMapper.MapFromReader(reader);
                return OperationResult<PatientDto>.Success(patient, "Patient created successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating patient: {Error}", ex.Message);
                return OperationResult<PatientDto>.Failure("Database operation failed");
            }
        }
    }
}
