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
    public static class CreatePrescriptionCommand
    {
        private const string CreatePrescriptionSql = @"
INSERT INTO dbo.prescriptions (appointment_id, instructions)
OUTPUT inserted.prescription_id, inserted.appointment_id, inserted.instructions
VALUES (@AppointmentId, @Instructions);
";

        public static async Task<OperationResult<PrescriptionDto>> ExecuteAsync(
            PrescriptionDto dto,
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("CreatePrescriptionCommand received null prescription data");
                return OperationResult<PrescriptionDto>.Failure("Prescription data is required");
            }

            logger.LogInformation("Executing prescription creation for AppointmentId: {AppointmentId}", dto.AppointmentId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, dto);
                using var reader = await command.ExecuteReaderAsync();

                return await ProcessResultAsync(reader, logger, dto.AppointmentId);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during prescription creation for AppointmentId: {AppointmentId}. Error: {Error}",
                    dto.AppointmentId, ex.Message);
                return OperationResult<PrescriptionDto>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during prescription creation for AppointmentId: {AppointmentId}", dto.AppointmentId);
                return OperationResult<PrescriptionDto>.Failure("Prescription creation failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, PrescriptionDto dto)
        {
            var command = new SqlCommand(CreatePrescriptionSql, connection);
            command.Parameters.AddWithValue("@AppointmentId", dto.AppointmentId);
            command.Parameters.AddWithValue("@Instructions", dto.Instructions ?? string.Empty);
            return command;
        }

        private static async Task<OperationResult<PrescriptionDto>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            int appointmentId)
        {
            if (!await reader.ReadAsync())
            {
                logger.LogWarning("No result returned from prescription creation for AppointmentId: {AppointmentId}", appointmentId);
                return OperationResult<PrescriptionDto>.Failure("Prescription creation returned no result");
            }

            var prescription = PrescriptionMapper.MapFromReader(reader);
            logger.LogInformation("Prescription created successfully - PrescriptionId: {PrescriptionId}", prescription.PrescriptionId);

            return OperationResult<PrescriptionDto>.Success(prescription, "Prescription created successfully");
        }
    }
}
