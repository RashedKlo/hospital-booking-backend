using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Clinic;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Clinic.Helpers;

namespace hospital_booking.Data.Repositories.Clinic.Commands
{
    public static class CreateClinicCommand
    {
        private const string CreateClinicSql = @"
INSERT INTO dbo.clinics (title, description, phone, address)
OUTPUT inserted.clinic_id, inserted.title, inserted.description, inserted.phone, inserted.address
VALUES (@Title, @Description, @Phone, @Address);
";

        public static async Task<OperationResult<ClinicDto>> ExecuteAsync(
            ClinicDto dto,
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("CreateClinicCommand received null clinic data");
                return OperationResult<ClinicDto>.Failure("Clinic data is required");
            }

            logger.LogInformation("Executing clinic creation for Title: {Title}", dto.Title);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, dto);
                using var reader = await command.ExecuteReaderAsync();

                return await ProcessResultAsync(reader, logger, dto.Title);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during clinic creation for {Title}. Error: {Error}",
                    dto.Title, ex.Message);
                return OperationResult<ClinicDto>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during clinic creation for {Title}", dto.Title);
                return OperationResult<ClinicDto>.Failure("Clinic creation failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, ClinicDto dto)
        {
            var command = new SqlCommand(CreateClinicSql, connection);
            command.Parameters.AddWithValue("@Title", dto.Title ?? string.Empty);
            command.Parameters.AddWithValue("@Description", dto.Description ?? string.Empty);
            command.Parameters.AddWithValue("@Phone", dto.Phone ?? string.Empty);
            command.Parameters.AddWithValue("@Address", dto.Address ?? string.Empty);
            return command;
        }

        private static async Task<OperationResult<ClinicDto>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            string Title)
        {
            if (!await reader.ReadAsync())
            {
                logger.LogWarning("No result returned from clinic creation for {Title}", Title);
                return OperationResult<ClinicDto>.Failure("Clinic creation returned no result");
            }

            var clinic = ClinicMapper.MapFromReader(reader);
            logger.LogInformation("Clinic created successfully - ClinicId: {ClinicId}", clinic.ClinicId);

            return OperationResult<ClinicDto>.Success(clinic, "Clinic created successfully");
        }
    }
}
