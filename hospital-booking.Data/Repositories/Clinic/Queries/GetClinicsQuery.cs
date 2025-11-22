using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Clinic;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Clinic.Helpers;

namespace hospital_booking.Data.Repositories.Clinic.Queries
{
    public class GetClinicsQuery
    {
        private const string GetClinicsSql = @"
    SELECT
        clinic_id,
        title,
        description,
        phone,
        address
    FROM dbo.clinics
    ORDER BY clinic_id
    OFFSET @Offset ROWS
    FETCH NEXT @Limit ROWS ONLY;
    ";

        public static async Task<OperationResult<List<ClinicDto>>> ExecuteAsync(
            int page,
            int limit,
            ILogger logger)
        {
            if (page <= 0)
            {
                logger.LogError("GetClinicsQuery received invalid page: {Page}", page);
                return OperationResult<List<ClinicDto>>.Failure("Page must be greater than 0");
            }

            if (limit <= 0 || limit > 1000)
            {
                logger.LogError("GetClinicsQuery received invalid limit: {Limit}", limit);
                return OperationResult<List<ClinicDto>>.Failure("Limit must be between 1 and 1000");
            }

            logger.LogInformation("Executing getting clinics with page: {Page}, limit: {Limit}", page, limit);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, page, limit);
                using var reader = await command.ExecuteReaderAsync();

                return await ProcessResultAsync(reader, logger, page, limit);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during getting clinics. Error: {Error}", ex.Message);
                return OperationResult<List<ClinicDto>>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during getting clinics");
                return OperationResult<List<ClinicDto>>.Failure("Getting clinics failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, int page, int limit)
        {
            var command = new SqlCommand(GetClinicsSql, connection);
            var offset = (page - 1) * limit;
            command.Parameters.AddWithValue("@Offset", offset);
            command.Parameters.AddWithValue("@Limit", limit);
            return command;
        }

        private static async Task<OperationResult<List<ClinicDto>>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            int page,
            int limit)
        {
            var clinics = new List<ClinicDto>();

            while (await reader.ReadAsync())
            {
                clinics.Add(ClinicMapper.MapFromReader(reader));
            }

            logger.LogInformation("Getting clinics successfully - Count: {Count}, Page: {Page}, Limit: {Limit}",
                clinics.Count, page, limit);

            return OperationResult<List<ClinicDto>>.Success(clinics, "Clinics retrieved successfully");
        }
    }
}
