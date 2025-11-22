using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Prescription;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Prescription.Helpers;

namespace hospital_booking.Data.Repositories.Prescription.Queries
{
    public class GetPrescriptionsQuery
    {
        private const string GetPrescriptionsSql = @"
    SELECT
        prescription_id,
        appointment_id,
        instructions
    FROM dbo.prescriptions
    ORDER BY prescription_id
    OFFSET @Offset ROWS
    FETCH NEXT @Limit ROWS ONLY;
    ";

        public static async Task<OperationResult<List<PrescriptionDto>>> ExecuteAsync(
            int page,
            int limit,
            ILogger logger)
        {
            if (page < 1 || limit < 1)
            {
                logger.LogError("GetPrescriptionsQuery received invalid pagination parameters - Page: {Page}, Limit: {Limit}", page, limit);
                return OperationResult<List<PrescriptionDto>>.Failure("Invalid pagination parameters");
            }

            logger.LogInformation("Executing getting prescriptions - Page: {Page}, Limit: {Limit}", page, limit);

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
                logger.LogError(ex, "Database error during getting prescriptions. Error: {Error}", ex.Message);
                return OperationResult<List<PrescriptionDto>>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during getting prescriptions");
                return OperationResult<List<PrescriptionDto>>.Failure("Getting prescriptions failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, int page, int limit)
        {
            var command = new SqlCommand(GetPrescriptionsSql, connection);
            command.Parameters.AddWithValue("@Offset", (page - 1) * limit);
            command.Parameters.AddWithValue("@Limit", limit);
            return command;
        }

        private static async Task<OperationResult<List<PrescriptionDto>>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            int page,
            int limit)
        {
            var prescriptions = new List<PrescriptionDto>();

            while (await reader.ReadAsync())
            {
                prescriptions.Add(PrescriptionMapper.MapFromReader(reader));
            }

            logger.LogInformation("Retrieved {Count} prescriptions - Page: {Page}, Limit: {Limit}",
                prescriptions.Count, page, limit);

            return OperationResult<List<PrescriptionDto>>.Success(prescriptions, "Prescriptions retrieved successfully");
        }
    }
}
