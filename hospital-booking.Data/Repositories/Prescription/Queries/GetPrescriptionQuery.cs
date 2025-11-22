using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Prescription;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Prescription.Helpers;

namespace hospital_booking.Data.Repositories.Prescription.Queries
{
    public class GetPrescriptionQuery
    {
        private const string GetPrescriptionSql = @"
    SELECT TOP (1)
        prescription_id,
        appointment_id,
        instructions
    FROM dbo.prescriptions
    WHERE prescription_id = @PrescriptionId;
    ";

        public static async Task<OperationResult<PrescriptionDto>> ExecuteAsync(
            int prescriptionId,
            ILogger logger)
        {
            if (prescriptionId <= 0)
            {
                logger.LogError("GetPrescriptionQuery received invalid prescription ID: {PrescriptionId}", prescriptionId);
                return OperationResult<PrescriptionDto>.Failure("Invalid prescription ID");
            }

            logger.LogInformation("Executing getting prescription by ID: {PrescriptionId}", prescriptionId);

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
                logger.LogError(ex, "Database error during getting prescription by PrescriptionId: {PrescriptionId}. Error: {Error}",
                    prescriptionId, ex.Message);
                return OperationResult<PrescriptionDto>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during getting prescription by PrescriptionId: {PrescriptionId}", prescriptionId);
                return OperationResult<PrescriptionDto>.Failure("Getting prescription failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, int prescriptionId)
        {
            var command = new SqlCommand(GetPrescriptionSql, connection);
            command.Parameters.AddWithValue("@PrescriptionId", prescriptionId);
            return command;
        }

        private static async Task<OperationResult<PrescriptionDto>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            int prescriptionId)
        {
            if (!await reader.ReadAsync())
            {
                logger.LogWarning("No result returned from getting prescription by PrescriptionId: {PrescriptionId}", prescriptionId);
                return OperationResult<PrescriptionDto>.Failure("Prescription not found");
            }

            var prescription = PrescriptionMapper.MapFromReader(reader);
            logger.LogInformation("Getting prescription successfully - PrescriptionId: {PrescriptionId}", prescription.PrescriptionId);

            return OperationResult<PrescriptionDto>.Success(prescription, "Prescription found successfully");
        }
    }
}
