using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.MedicalReport.Commands
{
    public static class DeleteMedicalReportCommand
    {
        private const string DeleteMedicalReportSql = @"
DELETE FROM dbo.medical_reports
WHERE report_id = @ReportId;
";

        public static async Task<OperationResult<bool>> ExecuteAsync(
            int reportId,
            ILogger logger)
        {
            if (reportId <= 0)
            {
                logger.LogError("DeleteMedicalReportCommand received invalid report ID: {ReportId}", reportId);
                return OperationResult<bool>.Failure("Invalid report ID");
            }

            logger.LogInformation("Executing medical report deletion for ReportId: {ReportId}", reportId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, reportId);
                var rowsAffected = await command.ExecuteNonQueryAsync();

                return ProcessResult(rowsAffected, logger, reportId);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during medical report deletion for ReportId: {ReportId}. Error: {Error}",
                    reportId, ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during medical report deletion for ReportId: {ReportId}", reportId);
                return OperationResult<bool>.Failure("Medical report deletion failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, int reportId)
        {
            var command = new SqlCommand(DeleteMedicalReportSql, connection);
            command.Parameters.AddWithValue("@ReportId", reportId);
            return command;
        }

        private static OperationResult<bool> ProcessResult(
            int rowsAffected,
            ILogger logger,
            int reportId)
        {
            if (rowsAffected == 0)
            {
                logger.LogWarning("No medical report found to delete for ReportId: {ReportId}", reportId);
                return OperationResult<bool>.Failure("Medical report not found");
            }

            logger.LogInformation("Medical report deleted successfully - ReportId: {ReportId}", reportId);
            return OperationResult<bool>.Success(true, "Medical report deleted successfully");
        }
    }
}
