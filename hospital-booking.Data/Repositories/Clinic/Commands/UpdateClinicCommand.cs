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
    public static class UpdateClinicCommand
    {
        private const string UpdateClinicSql = @"
UPDATE dbo.clinics
SET title = @Title,
    description = @Description,
    phone = @Phone,
    address = @Address
WHERE clinic_id = @ClinicId;

SELECT clinic_id, title, description, phone, address
FROM dbo.clinics
WHERE clinic_id = @ClinicId;
";

        public static async Task<OperationResult<ClinicDto>> ExecuteAsync(
            int clinicId,
            ClinicDto dto,
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("UpdateClinicCommand received null clinic data");
                return OperationResult<ClinicDto>.Failure("Clinic data is required");
            }

            logger.LogInformation("Executing clinic update for ClinicId: {ClinicId}", clinicId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, clinicId, dto);
                using var reader = await command.ExecuteReaderAsync();

                return await ProcessResultAsync(reader, logger, clinicId);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during clinic update for ClinicId: {ClinicId}. Error: {Error}",
                    clinicId, ex.Message);
                return OperationResult<ClinicDto>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during clinic update for ClinicId: {ClinicId}", clinicId);
                return OperationResult<ClinicDto>.Failure("Clinic update failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, int clinicId, ClinicDto dto)
        {
            var command = new SqlCommand(UpdateClinicSql, connection);
            command.Parameters.AddWithValue("@ClinicId", clinicId);
            command.Parameters.AddWithValue("@Title", dto.Title ?? string.Empty);
            command.Parameters.AddWithValue("@Description", dto.Description ?? string.Empty);
            command.Parameters.AddWithValue("@Phone", dto.Phone ?? string.Empty);
            command.Parameters.AddWithValue("@Address", dto.Address ?? string.Empty);
            return command;
        }

        private static async Task<OperationResult<ClinicDto>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            int ClinicId)
        {
            if (!await reader.ReadAsync())
            {
                logger.LogWarning("Clinic not found for ClinicId: {ClinicId}", ClinicId);
                return OperationResult<ClinicDto>.Failure("Clinic not found");
            }

            var clinic = ClinicMapper.MapFromReader(reader);
            logger.LogInformation("Clinic updated successfully - ClinicId: {ClinicId}", clinic.ClinicId);

            return OperationResult<ClinicDto>.Success(clinic, "Clinic updated successfully");
        }
    }
}
