using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.MedicalReport;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.MedicalReport.Helpers;

namespace hospital_booking.Data.Repositories.MedicalReport.Queries
{
    public class GetMedicalReportsQuery
    {
        private const string GetMedicalReportsSql = @"
    SELECT
        report_id,
        appointment_id,
        diagnosis,
        notes,
        required_tests
    FROM dbo.medical_reports
    ORDER BY report_id
    OFFSET @Offset ROWS
    FETCH NEXT @Limit ROWS ONLY;
    ";

        public static async Task<OperationResult<List<MedicalReportDto>>> ExecuteAsync(
            int page,
            int limit,
            ILogger logger)
        {
            if (page < 1 || limit < 1)
            {
                logger.LogError("GetMedicalReportsQuery received invalid pagination parameters - Page: {Page}, Limit: {Limit}", page, limit);
                return OperationResult<List<MedicalReportDto>>.Failure("Invalid pagination parameters");
            }

            logger.LogInformation("Executing getting medical reports - Page: {Page}, Limit: {Limit}", page, limit);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, page, limit);
                using var reader = await command.ExecuteReaderAsync();

                return await ProcessResultAsync(reader, logger, page, limit);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during getting medical reports. Error: {Error}", ex.Message);
                return OperationResult<List<MedicalReportDto>>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during getting medical reports");
                return OperationResult<List<MedicalReportDto>>.Failure("Getting medical reports failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, int page, int limit)
        {
            var command = new SqlCommand(GetMedicalReportsSql, connection);
            command.Parameters.AddWithValue("@Offset", (page - 1) * limit);
            command.Parameters.AddWithValue("@Limit", limit);
            return command;
        }

        private static async Task<OperationResult<List<MedicalReportDto>>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            int page,
            int limit)
        {
            var medicalReports = new List<MedicalReportDto>();

            while (await reader.ReadAsync())
            {
                medicalReports.Add(MedicalReportMapper.MapFromReader(reader));
            }

            logger.LogInformation("Retrieved {Count} medical reports - Page: {Page}, Limit: {Limit}",
                medicalReports.Count, page, limit);

            return OperationResult<List<MedicalReportDto>>.Success(medicalReports, "Medical reports retrieved successfully");
        }
    }
}
