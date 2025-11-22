using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Prescription;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Prescription.Helpers;

namespace hospital_booking.Data.Repositories.Prescription.Queries
{
    public class GetPrescriptionsByAppointmentQuery
    {
        private const string GetPrescriptionsByAppointmentSql = @"
    SELECT
        prescription_id,
        appointment_id,
        instructions
    FROM dbo.prescriptions
    WHERE appointment_id = @AppointmentId
    ORDER BY prescription_id;
    ";

        public static async Task<OperationResult<List<PrescriptionDto>>> ExecuteAsync(
            int appointmentId,
            ILogger logger)
        {
            if (appointmentId <= 0)
            {
                logger.LogError("GetPrescriptionsByAppointmentQuery received invalid appointment ID: {AppointmentId}", appointmentId);
                return OperationResult<List<PrescriptionDto>>.Failure("Invalid appointment ID");
            }

            logger.LogInformation("Executing getting prescriptions by AppointmentId: {AppointmentId}", appointmentId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, appointmentId);
                using var reader = await command.ExecuteReaderAsync();

                return await ProcessResultAsync(reader, logger, appointmentId);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during getting prescriptions by AppointmentId: {AppointmentId}. Error: {Error}",
                    appointmentId, ex.Message);
                return OperationResult<List<PrescriptionDto>>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during getting prescriptions by AppointmentId: {AppointmentId}", appointmentId);
                return OperationResult<List<PrescriptionDto>>.Failure("Getting prescriptions failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, int appointmentId)
        {
            var command = new SqlCommand(GetPrescriptionsByAppointmentSql, connection);
            command.Parameters.AddWithValue("@AppointmentId", appointmentId);
            return command;
        }

        private static async Task<OperationResult<List<PrescriptionDto>>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            int appointmentId)
        {
            var prescriptions = new List<PrescriptionDto>();

            while (await reader.ReadAsync())
            {
                prescriptions.Add(PrescriptionMapper.MapFromReader(reader));
            }

            logger.LogInformation("Retrieved {Count} prescriptions for AppointmentId: {AppointmentId}",
                prescriptions.Count, appointmentId);

            return OperationResult<List<PrescriptionDto>>.Success(prescriptions, "Prescriptions retrieved successfully");
        }
    }
}
