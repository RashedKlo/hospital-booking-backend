using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.PrescriptionItem;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.PrescriptionItem.Queries;

namespace hospital_booking.Data.Repositories.PrescriptionItem.Commands
{
    public static class UpdatePrescriptionItemCommand
    {
        private const string UpdateSql = @"
UPDATE dbo.prescription_items
SET name = ISNULL(@MedicationName, name),
    dosage = ISNULL(@Dosage, dosage),
    duration = ISNULL(@Duration, duration),
    frequency = ISNULL(@Frequency, frequency)
WHERE item_id = @ItemId;
";

        public static async Task<OperationResult<PrescriptionItemDto>> ExecuteAsync(int itemId, PrescriptionItemUpdateDto dto, ILogger logger)
        {
            if (itemId <= 0) return OperationResult<PrescriptionItemDto>.Failure("Invalid item ID");
            if (dto == null) return OperationResult<PrescriptionItemDto>.Failure("Data is required");

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(UpdateSql, connection);
                command.Parameters.AddWithValue("@ItemId", itemId);
                command.Parameters.AddWithValue("@MedicationName", (object?)dto.MedicationName ?? DBNull.Value);
                command.Parameters.AddWithValue("@Dosage", (object?)dto.Dosage ?? DBNull.Value);
                command.Parameters.AddWithValue("@Duration", (object?)dto.Duration ?? DBNull.Value);
                command.Parameters.AddWithValue("@Frequency", (object?)dto.Frequency ?? DBNull.Value);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows == 0)
                {
                    return OperationResult<PrescriptionItemDto>.Failure("Prescription item not found");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating prescription item: {Error}", ex.Message);
                return OperationResult<PrescriptionItemDto>.Failure("Database operation failed");
            }

            return await GetPrescriptionItemQuery.ExecuteAsync(itemId, logger);
        }
    }
}
