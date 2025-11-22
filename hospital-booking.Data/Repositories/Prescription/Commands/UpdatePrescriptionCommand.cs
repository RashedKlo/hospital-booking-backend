using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Prescription;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Prescription.Helpers;

namespace hospital_booking.Data.Repositories.Prescription.Commands
{
    public static class UpdatePrescriptionCommand
    {
        private const string UpdatePrescriptionSql = @"
UPDATE dbo.prescriptions
SET appointment_id = @AppointmentId,
    instructions = @Instructions
OUTPUT inserted.prescription_id, inserted.appointment_id, inserted.instructions
WHERE prescription_id = @PrescriptionId;
";

        public static async Task<OperationResult<PrescriptionDto>> ExecuteAsync(
            int prescriptionId,
            PrescriptionDto dto,
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("UpdatePrescriptionCommand received null prescription data");
                return OperationResult<PrescriptionDto>.Failure("Prescription data is required");
            }

            if (prescriptionId <= 0)
            {
                logger.LogError("UpdatePrescriptionCommand received invalid prescription ID: {PrescriptionId}", prescriptionId);
                return OperationResult<PrescriptionDto>.Failure("Invalid prescription ID");
            }

            logger.LogInformation("Executing prescription update for PrescriptionId: {PrescriptionId}", prescriptionId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, prescriptionId, dto);
                using var reader = await command.ExecuteReaderAsync();

                return await ProcessResultAsync(reader, logger, prescriptionId);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during prescription update for PrescriptionId: {PrescriptionId}. Error: {Error}",
                    prescriptionId, ex.Message);
                return OperationResult<PrescriptionDto>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during prescription update for PrescriptionId: {PrescriptionId}", prescriptionId);
                return OperationResult<PrescriptionDto>.Failure("Prescription update failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, int prescriptionId, PrescriptionDto dto)
        {
            var command = new SqlCommand(UpdatePrescriptionSql, connection);
            command.Parameters.AddWithValue("@PrescriptionId", prescriptionId);
            command.Parameters.AddWithValue("@AppointmentId", dto.AppointmentId);
            command.Parameters.AddWithValue("@Instructions", dto.Instructions ?? string.Empty);
            return command;
        }

        private static async Task<OperationResult<PrescriptionDto>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            int prescriptionId)
        {
            if (!await reader.ReadAsync())
            {
                logger.LogWarning("No result returned from prescription update for PrescriptionId: {PrescriptionId}", prescriptionId);
                return OperationResult<PrescriptionDto>.Failure("Prescription not found or update failed");
            }

            var prescription = PrescriptionMapper.MapFromReader(reader);
            logger.LogInformation("Prescription updated successfully - PrescriptionId: {PrescriptionId}", prescription.PrescriptionId);

            return OperationResult<PrescriptionDto>.Success(prescription, "Prescription updated successfully");
        }
    }
}
