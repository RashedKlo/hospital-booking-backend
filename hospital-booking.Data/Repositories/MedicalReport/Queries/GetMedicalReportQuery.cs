using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.MedicalReport;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.MedicalReport.Helpers;

namespace hospital_booking.Data.Repositories.MedicalReport.Queries
{
    public class GetMedicalReportQuery
    {
        private const string GetMedicalReportSql = @"
    SELECT TOP (1)
        report_id,
        appointment_id,
        diagnosis,
        notes,
        required_tests
    FROM dbo.medical_reports
    WHERE report_id = @ReportId;
    ";

        public static async Task<OperationResult<MedicalReportDto>> ExecuteAsync(
            int reportId,
            ILogger logger)
        {
            if (reportId <= 0)
            {
                logger.LogError("GetMedicalReportQuery received invalid report ID: {ReportId}", reportId);
                return OperationResult<MedicalReportDto>.Failure("Invalid report ID");
            }

            logger.LogInformation("Executing getting medical report by ID: {ReportId}", reportId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, reportId);
                using var reader = await command.ExecuteReaderAsync();

                return await ProcessResultAsync(reader, logger, reportId);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during getting medical report by ReportId: {ReportId}. Error: {Error}",
                    reportId, ex.Message);
                return OperationResult<MedicalReportDto>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during getting medical report by ReportId: {ReportId}", reportId);
                return OperationResult<MedicalReportDto>.Failure("Getting medical report failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, int reportId)
        {
            var command = new SqlCommand(GetMedicalReportSql, connection);
            command.Parameters.AddWithValue("@ReportId", reportId);
            return command;
        }

        private static async Task<OperationResult<MedicalReportDto>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            int reportId)
        {
            if (!await reader.ReadAsync())
            {
                logger.LogWarning("No result returned from getting medical report by ReportId: {ReportId}", reportId);
                return OperationResult<MedicalReportDto>.Failure("Medical report not found");
            }

            var medicalReport = MedicalReportMapper.MapFromReader(reader);
            logger.LogInformation("Getting medical report successfully - ReportId: {ReportId}", medicalReport.ReportId);

            return OperationResult<MedicalReportDto>.Success(medicalReport, "Medical report found successfully");
        }
    }
}
