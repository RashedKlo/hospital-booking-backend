using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.ClinicService.Commands
{
    public static class DeleteServiceCommand
    {
        private const string DeleteSql = "DELETE FROM dbo.clinic_services WHERE service_id = @ServiceId;";

        public static async Task<OperationResult<bool>> ExecuteAsync(int serviceId, ILogger logger)
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(DeleteSql, connection);
                command.Parameters.AddWithValue("@ServiceId", serviceId);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    return OperationResult<bool>.Success(true, "Service deleted successfully");
                }
                return OperationResult<bool>.Failure("Service not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting service {ServiceId}: {Error}", serviceId, ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
        }
    }
}
