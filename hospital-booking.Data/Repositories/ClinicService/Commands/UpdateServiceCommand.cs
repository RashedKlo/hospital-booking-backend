using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.ClinicService;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.ClinicService.Queries;

namespace hospital_booking.Data.Repositories.ClinicService.Commands
{
    public static class UpdateServiceCommand
    {
        public static async Task<OperationResult<ClinicServiceDto>> ExecuteAsync(int serviceId, ClinicServiceUpdateDto dto, ILogger logger)
        {
            if (dto == null)
            {
                return OperationResult<ClinicServiceDto>.Failure("Update data is required");
            }

            try
            {
                var updates = new List<string>();
                var parameters = new List<SqlParameter>();

                if (dto.Title != null)
                {
                    updates.Add("title = @Title");
                    parameters.Add(new SqlParameter("@Title", dto.Title));
                }

                if (dto.Description != null)
                {
                    updates.Add("description = @Description");
                    parameters.Add(new SqlParameter("@Description", (object)dto.Description ?? DBNull.Value));
                }

                if (dto.Price.HasValue)
                {
                    updates.Add("price = @Price");
                    parameters.Add(new SqlParameter("@Price", dto.Price.Value));
                }

                if (!updates.Any())
                {
                    return await GetServiceQuery.ExecuteAsync(serviceId, logger);
                }

                updates.Add("updated_at = GETDATE()");

                var sql = $@"
UPDATE dbo.clinic_services 
SET {string.Join(", ", updates)}
WHERE service_id = @ServiceId;
";
                parameters.Add(new SqlParameter("@ServiceId", serviceId));

                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddRange(parameters.ToArray());

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    return await GetServiceQuery.ExecuteAsync(serviceId, logger);
                }
                return OperationResult<ClinicServiceDto>.Failure("Service not found or no changes made");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating service {ServiceId}: {Error}", serviceId, ex.Message);
                return OperationResult<ClinicServiceDto>.Failure("Database operation failed");
            }
        }
    }
}
