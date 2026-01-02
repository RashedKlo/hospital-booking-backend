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
        private const string GetSql = @"
SELECT 
    pi.item_id, pi.prescription_id, pi.name, pi.dosage, pi.duration, pi.frequency,
    p.prescription_id, p.appointment_id, p.instructions
FROM dbo.prescription_items pi
INNER JOIN dbo.prescriptions p ON pi.prescription_id = p.prescription_id
WHERE pi.item_id = @ItemId;
";

        public static async Task<OperationResult<PrescriptionItemDto>> ExecuteAsync(int itemId, ILogger logger)
        {
            if (itemId <= 0)
            {
                return OperationResult<PrescriptionItemDto>.Failure("Invalid item ID");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetSql, connection);
                command.Parameters.AddWithValue("@ItemId", itemId);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return OperationResult<PrescriptionItemDto>.Failure("Prescription item not found");
                }

                var dto = PrescriptionItemMapper.MapFromReader(reader);
                return OperationResult<PrescriptionItemDto>.Success(dto, "Prescription item retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting prescription item: {Error}", ex.Message);
                return OperationResult<PrescriptionItemDto>.Failure("Database operation failed");
            }
        }
    }
}
