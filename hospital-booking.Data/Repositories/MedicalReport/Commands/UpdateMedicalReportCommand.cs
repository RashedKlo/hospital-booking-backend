using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.MedicalReport;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.MedicalReport.Queries;

namespace hospital_booking.Data.Repositories.MedicalReport.Commands
{
    public static class UpdateMedicalReportCommand
    {
        private const string UpdateSql = @"
UPDATE dbo.medical_reports
SET diagnosis = ISNULL(@Diagnosis, diagnosis),
    notes = ISNULL(@Notes, notes),
    required_tests = ISNULL(@RequiredTests, required_tests)
WHERE report_id = @ReportId;
";

        public static async Task<OperationResult<MedicalReportDto>> ExecuteAsync(int reportId, MedicalReportUpdateDto dto, ILogger logger)
        {
            if (reportId <= 0) return OperationResult<MedicalReportDto>.Failure("Invalid report ID");
            if (dto == null) return OperationResult<MedicalReportDto>.Failure("Data is required");

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(UpdateSql, connection);
                command.Parameters.AddWithValue("@ReportId", reportId);
                command.Parameters.AddWithValue("@Diagnosis", (object?)dto.Diagnosis ?? DBNull.Value);
                command.Parameters.AddWithValue("@Notes", (object?)dto.Notes ?? DBNull.Value);
                command.Parameters.AddWithValue("@RequiredTests", (object?)dto.RequiredTests ?? DBNull.Value);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows == 0)
                {
                    return OperationResult<MedicalReportDto>.Failure("Medical report not found");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating medical report: {Error}", ex.Message);
                return OperationResult<MedicalReportDto>.Failure("Database operation failed");
            }

            return await GetMedicalReportQuery.ExecuteAsync(reportId, logger);
        }
    }
}
