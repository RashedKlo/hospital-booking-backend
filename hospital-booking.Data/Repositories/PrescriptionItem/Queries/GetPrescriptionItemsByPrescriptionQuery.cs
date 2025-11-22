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
    public class GetPrescriptionItemsByPrescriptionQuery
    {
        private const string GetPrescriptionItemsByPrescriptionSql = @"
    SELECT
        item_id,
        prescription_id,
        name,
        dosage,
        duration,
        frequency
    FROM dbo.prescription_items
    WHERE prescription_id = @PrescriptionId
    ORDER BY item_id;
    ";

        public static async Task<OperationResult<List<PrescriptionItemDto>>> ExecuteAsync(
            int prescriptionId,
            ILogger logger)
        {
            if (prescriptionId <= 0)
            {
                logger.LogError("GetPrescriptionItemsByPrescriptionQuery received invalid prescription ID: {PrescriptionId}", prescriptionId);
                return OperationResult<List<PrescriptionItemDto>>.Failure("Invalid prescription ID");
            }

            logger.LogInformation("Executing getting prescription items by PrescriptionId: {PrescriptionId}", prescriptionId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, prescriptionId);
                using var reader = await command.ExecuteReaderAsync();

                return await ProcessResultAsync(reader, logger, prescriptionId);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during getting prescription items by PrescriptionId: {PrescriptionId}. Error: {Error}",
                    prescriptionId, ex.Message);
                return OperationResult<List<PrescriptionItemDto>>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during getting prescription items by PrescriptionId: {PrescriptionId}", prescriptionId);
                return OperationResult<List<PrescriptionItemDto>>.Failure("Getting prescription items failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, int prescriptionId)
        {
            var command = new SqlCommand(GetPrescriptionItemsByPrescriptionSql, connection);
            command.Parameters.AddWithValue("@PrescriptionId", prescriptionId);
            return command;
        }

        private static async Task<OperationResult<List<PrescriptionItemDto>>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            int prescriptionId)
        {
            var prescriptionItems = new List<PrescriptionItemDto>();

            while (await reader.ReadAsync())
            {
                prescriptionItems.Add(PrescriptionItemMapper.MapFromReader(reader));
            }

            logger.LogInformation("Retrieved {Count} prescription items for PrescriptionId: {PrescriptionId}",
                prescriptionItems.Count, prescriptionId);

            return OperationResult<List<PrescriptionItemDto>>.Success(prescriptionItems, "Prescription items retrieved successfully");
        }
    }
}
