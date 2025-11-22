using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.PrescriptionItem.Commands
{
    public static class DeletePrescriptionItemCommand
    {
        private const string DeletePrescriptionItemSql = @"
DELETE FROM dbo.prescription_items
WHERE item_id = @ItemId;
";

        public static async Task<OperationResult<bool>> ExecuteAsync(
            int itemId,
            ILogger logger)
        {
            if (itemId <= 0)
            {
                logger.LogError("DeletePrescriptionItemCommand received invalid item ID: {ItemId}", itemId);
                return OperationResult<bool>.Failure("Invalid item ID");
            }

            logger.LogInformation("Executing prescription item deletion for ItemId: {ItemId}", itemId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, itemId);
                var rowsAffected = await command.ExecuteNonQueryAsync();

                return ProcessResult(rowsAffected, logger, itemId);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during prescription item deletion for ItemId: {ItemId}. Error: {Error}",
                    itemId, ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during prescription item deletion for ItemId: {ItemId}", itemId);
                return OperationResult<bool>.Failure("Prescription item deletion failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, int itemId)
        {
            var command = new SqlCommand(DeletePrescriptionItemSql, connection);
            command.Parameters.AddWithValue("@ItemId", itemId);
            return command;
        }

        private static OperationResult<bool> ProcessResult(
            int rowsAffected,
            ILogger logger,
            int itemId)
        {
            if (rowsAffected == 0)
            {
                logger.LogWarning("No prescription item found to delete for ItemId: {ItemId}", itemId);
                return OperationResult<bool>.Failure("Prescription item not found");
            }

            logger.LogInformation("Prescription item deleted successfully - ItemId: {ItemId}", itemId);
            return OperationResult<bool>.Success(true, "Prescription item deleted successfully");
        }
    }
}
