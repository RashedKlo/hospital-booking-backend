using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.PrescriptionItem;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.PrescriptionItem.Helpers;

namespace hospital_booking.Data.Repositories.PrescriptionItem.Queries
{
    public class GetPrescriptionItemsQuery
    {
        private const string GetPrescriptionItemsSql = @"
    SELECT
        item_id,
        prescription_id,
        name,
        dosage,
        duration,
        frequency
    FROM dbo.prescription_items
    ORDER BY item_id
    OFFSET @Offset ROWS
    FETCH NEXT @Limit ROWS ONLY;
    ";

        public static async Task<OperationResult<List<PrescriptionItemDto>>> ExecuteAsync(
            int page,
            int limit,
            ILogger logger)
        {
            if (page < 1 || limit < 1)
            {
                logger.LogError("GetPrescriptionItemsQuery received invalid pagination parameters - Page: {Page}, Limit: {Limit}", page, limit);
                return OperationResult<List<PrescriptionItemDto>>.Failure("Invalid pagination parameters");
            }

            logger.LogInformation("Executing getting prescription items - Page: {Page}, Limit: {Limit}", page, limit);

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
                logger.LogError(ex, "Database error during getting prescription items. Error: {Error}", ex.Message);
                return OperationResult<List<PrescriptionItemDto>>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during getting prescription items");
                return OperationResult<List<PrescriptionItemDto>>.Failure("Getting prescription items failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, int page, int limit)
        {
            var command = new SqlCommand(GetPrescriptionItemsSql, connection);
            command.Parameters.AddWithValue("@Offset", (page - 1) * limit);
            command.Parameters.AddWithValue("@Limit", limit);
            return command;
        }

        private static async Task<OperationResult<List<PrescriptionItemDto>>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            int page,
            int limit)
        {
            var prescriptionItems = new List<PrescriptionItemDto>();

            while (await reader.ReadAsync())
            {
                prescriptionItems.Add(PrescriptionItemMapper.MapFromReader(reader));
            }

            logger.LogInformation("Retrieved {Count} prescription items - Page: {Page}, Limit: {Limit}",
                prescriptionItems.Count, page, limit);

            return OperationResult<List<PrescriptionItemDto>>.Success(prescriptionItems, "Prescription items retrieved successfully");
        }
    }
}
