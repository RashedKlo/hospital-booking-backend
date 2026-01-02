using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.ClinicFacility;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.ClinicFacility.Commands
{
    public static class CreateFacilityCommand
    {
        private const string CreateSql = @"
INSERT INTO dbo.clinic_facilities (clinic_id, title, created_at, updated_at)
VALUES (@ClinicId, @Title, GETDATE(), GETDATE());
";

        public static async Task<OperationResult<bool>> ExecuteAsync(ClinicFacilityAddDto dto, ILogger logger)
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(CreateSql, connection);
                command.Parameters.AddWithValue("@ClinicId", dto.ClinicId);
                command.Parameters.AddWithValue("@Title", dto.Title);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    return OperationResult<bool>.Success(true, "Facility added successfully");
                }
                return OperationResult<bool>.Failure("Failed to add facility");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating facility: {Error}", ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
        }
    }
}
