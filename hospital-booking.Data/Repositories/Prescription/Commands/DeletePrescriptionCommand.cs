using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Prescription.Commands
{
    public static class DeletePrescriptionCommand
    {
        private const string DeletePrescriptionSql = @"
DELETE FROM dbo.prescriptions
WHERE prescription_id = @PrescriptionId;
";

        public static async Task<OperationResult<bool>> ExecuteAsync(
            int prescriptionId,
            ILogger logger)
        {
            if (prescriptionId <= 0)
            {
                logger.LogError("DeletePrescriptionCommand received invalid prescription ID: {PrescriptionId}", prescriptionId);
                return OperationResult<bool>.Failure("Invalid prescription ID");
            }

            logger.LogInformation("Executing prescription deletion for PrescriptionId: {PrescriptionId}", prescriptionId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, prescriptionId);
                var rowsAffected = await command.ExecuteNonQueryAsync();

                return ProcessResult(rowsAffected, logger, prescriptionId);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during prescription deletion for PrescriptionId: {PrescriptionId}. Error: {Error}",
                    prescriptionId, ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during prescription deletion for PrescriptionId: {PrescriptionId}", prescriptionId);
                return OperationResult<bool>.Failure("Prescription deletion failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, int prescriptionId)
        {
            var command = new SqlCommand(DeletePrescriptionSql, connection);
            command.Parameters.AddWithValue("@PrescriptionId", prescriptionId);
            return command;
        }

        private static OperationResult<bool> ProcessResult(
            int rowsAffected,
            ILogger logger,
            int prescriptionId)
        {
            if (rowsAffected == 0)
            {
                logger.LogWarning("No prescription found to delete for PrescriptionId: {PrescriptionId}", prescriptionId);
                return OperationResult<bool>.Failure("Prescription not found");
            }

            logger.LogInformation("Prescription deleted successfully - PrescriptionId: {PrescriptionId}", prescriptionId);
            return OperationResult<bool>.Success(true, "Prescription deleted successfully");
        }
    }
}
