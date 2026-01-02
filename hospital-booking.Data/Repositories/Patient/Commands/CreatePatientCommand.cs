using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Patient.Commands
{
    public static class CreatePatientCommand
    {
        private const string CreateSql = @"
INSERT INTO dbo.patients (user_id, full_name, birthDate, gender, notes)
VALUES (@UserId, @FullName, @BirthDate, @Gender, @Notes);
";

        public static async Task<OperationResult<bool>> ExecuteAsync(PatientAddDto dto, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("CreatePatientCommand received null dto");
                return OperationResult<bool>.Failure("Patient data is required");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(CreateSql, connection);
                command.Parameters.AddWithValue("@UserId", (object?)dto.UserId ?? DBNull.Value);
                command.Parameters.AddWithValue("@FullName", dto.FullName);
                command.Parameters.AddWithValue("@BirthDate", (object?)dto.BirthDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@Gender", (object?)dto.Gender ?? DBNull.Value);
                command.Parameters.AddWithValue("@Notes", (object?)dto.Notes ?? DBNull.Value);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    return OperationResult<bool>.Success(true, "Patient created successfully");
                }
                return OperationResult<bool>.Failure("Failed to create patient");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating patient: {Error}", ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
        }
    }
}
