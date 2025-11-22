using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.PrescriptionItem;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.PrescriptionItem.Helpers;

namespace hospital_booking.Data.Repositories.PrescriptionItem.Commands
{
    public static class UpdatePrescriptionItemCommand
    {
        private const string UpdatePrescriptionItemSql = @"
UPDATE dbo.prescription_items
SET prescription_id = @PrescriptionId,
    name = @Name,
    dosage = @Dosage,
    duration = @Duration,
    frequency = @Frequency
OUTPUT inserted.item_id, inserted.prescription_id, inserted.name, inserted.dosage, inserted.duration, inserted.frequency
WHERE item_id = @ItemId;
";

        public static async Task<OperationResult<PrescriptionItemDto>> ExecuteAsync(
            int itemId,
            PrescriptionItemDto dto,
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("UpdatePrescriptionItemCommand received null prescription item data");
                return OperationResult<PrescriptionItemDto>.Failure("Prescription item data is required");
            }

            if (itemId <= 0)
            {
                logger.LogError("UpdatePrescriptionItemCommand received invalid item ID: {ItemId}", itemId);
                return OperationResult<PrescriptionItemDto>.Failure("Invalid item ID");
            }

            logger.LogInformation("Executing prescription item update for ItemId: {ItemId}", itemId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, itemId, dto);
                using var reader = await command.ExecuteReaderAsync();

                return await ProcessResultAsync(reader, logger, itemId);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during prescription item update for ItemId: {ItemId}. Error: {Error}",
                    itemId, ex.Message);
                return OperationResult<PrescriptionItemDto>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during prescription item update for ItemId: {ItemId}", itemId);
                return OperationResult<PrescriptionItemDto>.Failure("Prescription item update failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, int itemId, PrescriptionItemDto dto)
        {
            var command = new SqlCommand(UpdatePrescriptionItemSql, connection);
            command.Parameters.AddWithValue("@ItemId", itemId);
            command.Parameters.AddWithValue("@PrescriptionId", dto.PrescriptionId);
            command.Parameters.AddWithValue("@Name", dto.Name ?? string.Empty);
            command.Parameters.AddWithValue("@Dosage", dto.Dosage ?? string.Empty);
            command.Parameters.AddWithValue("@Duration", dto.Duration ?? string.Empty);
            command.Parameters.AddWithValue("@Frequency", dto.Frequency ?? string.Empty);
            return command;
        }

        private static async Task<OperationResult<PrescriptionItemDto>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            int itemId)
        {
            if (!await reader.ReadAsync())
            {
                logger.LogWarning("No result returned from prescription item update for ItemId: {ItemId}", itemId);
                return OperationResult<PrescriptionItemDto>.Failure("Prescription item not found or update failed");
            }

            var prescriptionItem = PrescriptionItemMapper.MapFromReader(reader);
            logger.LogInformation("Prescription item updated successfully - ItemId: {ItemId}", prescriptionItem.ItemId);

            return OperationResult<PrescriptionItemDto>.Success(prescriptionItem, "Prescription item updated successfully");
        }
    }
}
