using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Prescription;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Prescription.Queries;

namespace hospital_booking.Data.Repositories.Prescription.Commands
{
    public static class UpdatePrescriptionCommand
    {
        private const string UpdateSql = @"
UPDATE dbo.prescriptions
SET instructions = ISNULL(@Instructions, instructions)
WHERE prescription_id = @PrescriptionId;
";

        public static async Task<OperationResult<PrescriptionDto>> ExecuteAsync(int prescriptionId, PrescriptionUpdateDto dto, ILogger logger)
        {
            if (prescriptionId <= 0) return OperationResult<PrescriptionDto>.Failure("Invalid prescription ID");
            if (dto == null) return OperationResult<PrescriptionDto>.Failure("Data is required");

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(UpdateSql, connection);
                command.Parameters.AddWithValue("@PrescriptionId", prescriptionId);
                command.Parameters.AddWithValue("@Instructions", (object?)dto.Instructions ?? DBNull.Value);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows == 0)
                {
                    return OperationResult<PrescriptionDto>.Failure("Prescription not found");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating prescription: {Error}", ex.Message);
                return OperationResult<PrescriptionDto>.Failure("Database operation failed");
            }

            return await GetPrescriptionQuery.ExecuteAsync(prescriptionId, logger);
        }
    }
}
