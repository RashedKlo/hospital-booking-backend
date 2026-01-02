using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.MedicalReport;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.MedicalReport.Commands
{
    public static class CreateMedicalReportCommand
    {
        private const string CreateSql = @"
INSERT INTO dbo.medical_reports (appointment_id, diagnosis, notes, required_tests)
VALUES (@AppointmentId, @Diagnosis, @Notes, @RequiredTests);
";

        public static async Task<OperationResult<bool>> ExecuteAsync(MedicalReportAddDto dto, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("CreateMedicalReportCommand received null dto");
                return OperationResult<bool>.Failure("Medical report data is required");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(CreateSql, connection);
                command.Parameters.AddWithValue("@AppointmentId", dto.AppointmentId);
                command.Parameters.AddWithValue("@Diagnosis", (object?)dto.Diagnosis ?? DBNull.Value);
                command.Parameters.AddWithValue("@Notes", (object?)dto.Notes ?? DBNull.Value);
                command.Parameters.AddWithValue("@RequiredTests", (object?)dto.RequiredTests ?? DBNull.Value);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    return OperationResult<bool>.Success(true, "Medical report created successfully");
                }
                return OperationResult<bool>.Failure("Failed to create medical report");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating medical report: {Error}", ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
        }
    }
}
