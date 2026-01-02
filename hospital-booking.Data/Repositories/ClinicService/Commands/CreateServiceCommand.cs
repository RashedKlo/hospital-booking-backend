using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.ClinicService;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.ClinicService.Commands
{
    public static class CreateServiceCommand
    {
        private const string CreateSql = @"
INSERT INTO dbo.clinic_services (
    clinic_id, title, description, price, created_at, updated_at
)
VALUES (
    @ClinicId, @Title, @Description, @Price, GETDATE(), GETDATE()
);
";

        public static async Task<OperationResult<bool>> ExecuteAsync(ClinicServiceAddDto dto, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("CreateServiceCommand received null dto");
                return OperationResult<bool>.Failure("Service data is required");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(CreateSql, connection);
                command.Parameters.AddWithValue("@ClinicId", dto.ClinicId);
                command.Parameters.AddWithValue("@Title", dto.Title);
                command.Parameters.AddWithValue("@Description", (object?)dto.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@Price", dto.Price);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    return OperationResult<bool>.Success(true, "Service created successfully");
                }
                return OperationResult<bool>.Failure("Failed to create service");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating service: {Error}", ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
        }
    }
}
