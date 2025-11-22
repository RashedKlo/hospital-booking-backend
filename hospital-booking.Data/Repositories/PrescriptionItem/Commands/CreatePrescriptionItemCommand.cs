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
    public static class CreatePrescriptionItemCommand
    {
        private const string CreatePrescriptionItemSql = @"
INSERT INTO dbo.prescription_items (prescription_id, name, dosage, duration, frequency)
OUTPUT inserted.item_id, inserted.prescription_id, inserted.name, inserted.dosage, inserted.duration, inserted.frequency
VALUES (@PrescriptionId, @Name, @Dosage, @Duration, @Frequency);
";

        public static async Task<OperationResult<PrescriptionItemDto>> ExecuteAsync(
            PrescriptionItemDto dto,
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("CreatePrescriptionItemCommand received null prescription item data");
                return OperationResult<PrescriptionItemDto>.Failure("Prescription item data is required");
            }

            logger.LogInformation("Executing prescription item creation for PrescriptionId: {PrescriptionId}", dto.PrescriptionId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, dto);
                using var reader = await command.ExecuteReaderAsync();

                return await ProcessResultAsync(reader, logger, dto.PrescriptionId);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during prescription item creation for PrescriptionId: {PrescriptionId}. Error: {Error}",
                    dto.PrescriptionId, ex.Message);
                return OperationResult<PrescriptionItemDto>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during prescription item creation for PrescriptionId: {PrescriptionId}", dto.PrescriptionId);
                return OperationResult<PrescriptionItemDto>.Failure("Prescription item creation failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, PrescriptionItemDto dto)
        {
            var command = new SqlCommand(CreatePrescriptionItemSql, connection);
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
            int prescriptionId)
        {
            if (!await reader.ReadAsync())
            {
                logger.LogWarning("No result returned from prescription item creation for PrescriptionId: {PrescriptionId}", prescriptionId);
                return OperationResult<PrescriptionItemDto>.Failure("Prescription item creation returned no result");
            }

            var prescriptionItem = PrescriptionItemMapper.MapFromReader(reader);
            logger.LogInformation("Prescription item created successfully - ItemId: {ItemId}", prescriptionItem.ItemId);

            return OperationResult<PrescriptionItemDto>.Success(prescriptionItem, "Prescription item created successfully");
        }
    }
}
