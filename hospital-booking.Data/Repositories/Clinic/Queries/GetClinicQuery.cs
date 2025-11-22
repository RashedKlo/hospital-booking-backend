using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Clinic;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Clinic.Helpers;

namespace hospital_booking.Data.Repositories.Clinic.Queries
{
    public class GetClinicQuery
    {
        private const string GetClinicSql = @"
    SELECT TOP (1)
        clinic_id,
        title,
        description,
        phone,
        address
    FROM dbo.clinics
    WHERE clinic_id = @ClinicId;
    ";

        public static async Task<OperationResult<ClinicDto>> ExecuteAsync(
            int clinicId,
            ILogger logger)
        {
            if (clinicId <= 0)
            {
                logger.LogError("GetClinicQuery received invalid clinic ID: {ClinicId}", clinicId);
                return OperationResult<ClinicDto>.Failure("Invalid clinic ID");
            }

            logger.LogInformation("Executing getting clinic by ID: {ClinicId}", clinicId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, clinicId);
                using var reader = await command.ExecuteReaderAsync();

                return await ProcessResultAsync(reader, logger, clinicId);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during getting clinic by ClinicId: {ClinicId}. Error: {Error}",
                    clinicId, ex.Message);
                return OperationResult<ClinicDto>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during getting clinic by ClinicId: {ClinicId}", clinicId);
                return OperationResult<ClinicDto>.Failure("Getting clinic failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, int clinicId)
        {
            var command = new SqlCommand(GetClinicSql, connection);
            command.Parameters.AddWithValue("@ClinicId", clinicId);
            return command;
        }

        private static async Task<OperationResult<ClinicDto>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            int clinicId)
        {
            if (!await reader.ReadAsync())
            {
                logger.LogWarning("No result returned from getting clinic by ClinicId: {ClinicId}", clinicId);
                return OperationResult<ClinicDto>.Failure("Clinic not found");
            }

            var clinic = ClinicMapper.MapFromReader(reader);
            logger.LogInformation("Getting clinic successfully - ClinicId: {ClinicId}", clinic.ClinicId);

            return OperationResult<ClinicDto>.Success(clinic, "Clinic found successfully");
        }
    }
}
