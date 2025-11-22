using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.PrescriptionItem;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.PrescriptionItem.Helpers;

namespace hospital_booking.Data.Repositories.PrescriptionItem.Queries
{
    public class GetPrescriptionItemQuery
    {
        private const string GetPrescriptionItemSql = @"
    SELECT TOP (1)
        item_id,
        prescription_id,
        name,
        dosage,
        duration,
        frequency
    FROM dbo.prescription_items
    WHERE item_id = @ItemId;
    ";

        public static async Task<OperationResult<PrescriptionItemDto>> ExecuteAsync(
            int itemId,
            ILogger logger)
        {
            if (itemId <= 0)
            {
                logger.LogError("GetPrescriptionItemQuery received invalid item ID: {ItemId}", itemId);
                return OperationResult<PrescriptionItemDto>.Failure("Invalid item ID");
            }

            logger.LogInformation("Executing getting prescription item by ID: {ItemId}", itemId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, itemId);
                using var reader = await command.ExecuteReaderAsync();

                return await ProcessResultAsync(reader, logger, itemId);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during getting prescription item by ItemId: {ItemId}. Error: {Error}",
                    itemId, ex.Message);
                return OperationResult<PrescriptionItemDto>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during getting prescription item by ItemId: {ItemId}", itemId);
                return OperationResult<PrescriptionItemDto>.Failure("Getting prescription item failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, int itemId)
        {
            var command = new SqlCommand(GetPrescriptionItemSql, connection);
            command.Parameters.AddWithValue("@ItemId", itemId);
            return command;
        }

        private static async Task<OperationResult<PrescriptionItemDto>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            int itemId)
        {
            if (!await reader.ReadAsync())
            {
                logger.LogWarning("No result returned from getting prescription item by ItemId: {ItemId}", itemId);
                return OperationResult<PrescriptionItemDto>.Failure("Prescription item not found");
            }

            var prescriptionItem = PrescriptionItemMapper.MapFromReader(reader);
            logger.LogInformation("Getting prescription item successfully - ItemId: {ItemId}", prescriptionItem.ItemId);

            return OperationResult<PrescriptionItemDto>.Success(prescriptionItem, "Prescription item found successfully");
        }
    }
}
