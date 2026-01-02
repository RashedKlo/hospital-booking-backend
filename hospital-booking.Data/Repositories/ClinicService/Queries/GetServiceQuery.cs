using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.ClinicService;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.ClinicService.Queries
{
    public static class GetServiceQuery
    {
        private const string SelectSql = @"
SELECT service_id, clinic_id, title, description, price, created_at, updated_at
FROM dbo.clinic_services
WHERE service_id = @ServiceId;
";

        public static async Task<OperationResult<ClinicServiceDto>> ExecuteAsync(int serviceId, ILogger logger)
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(SelectSql, connection);
                command.Parameters.AddWithValue("@ServiceId", serviceId);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var dto = MapToDto(reader);
                    return OperationResult<ClinicServiceDto>.Success(dto);
                }
                return OperationResult<ClinicServiceDto>.Failure("Service not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting service {ServiceId}: {Error}", serviceId, ex.Message);
                return OperationResult<ClinicServiceDto>.Failure("Database operation failed");
            }
        }

        public static ClinicServiceDto MapToDto(IDataReader reader)
        {
            return new ClinicServiceDto
            {
                ServiceId = Convert.ToInt32(reader["service_id"]),
                ClinicId = Convert.ToInt32(reader["clinic_id"]),
                Title = reader["title"].ToString() ?? string.Empty,
                Description = reader["description"]?.ToString(),
                Price = Convert.ToDecimal(reader["price"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"]),
                UpdatedAt = Convert.ToDateTime(reader["updated_at"])
            };
        }
    }
}
