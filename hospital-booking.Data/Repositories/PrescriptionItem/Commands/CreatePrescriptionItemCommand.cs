using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.PrescriptionItem;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.PrescriptionItem.Commands
{
    public static class CreatePrescriptionItemCommand
    {
        private const string CreateSql = @"
INSERT INTO dbo.prescription_items (prescription_id, name, dosage, duration, frequency)
VALUES (@PrescriptionId, @MedicationName, @Dosage, @Duration, @Frequency);
";

        public static async Task<OperationResult<bool>> ExecuteAsync(PrescriptionItemAddDto dto, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("CreatePrescriptionItemCommand received null dto");
                return OperationResult<bool>.Failure("Prescription item data is required");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(CreateSql, connection);
                command.Parameters.AddWithValue("@PrescriptionId", dto.PrescriptionId);
                command.Parameters.AddWithValue("@MedicationName", dto.MedicationName ?? string.Empty);
                command.Parameters.AddWithValue("@Dosage", dto.Dosage ?? string.Empty);
                command.Parameters.AddWithValue("@Duration", dto.Duration ?? string.Empty);
                command.Parameters.AddWithValue("@Frequency", dto.Frequency ?? string.Empty);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    return OperationResult<bool>.Success(true, "Prescription item created successfully");
                }
                return OperationResult<bool>.Failure("Failed to create prescription item");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating prescription item: {Error}", ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
        }
    }
}
