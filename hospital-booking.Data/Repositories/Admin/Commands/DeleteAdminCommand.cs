using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Admin.Commands
{
    public static class DeleteAdminCommand
    {
        private const string DeleteSql = @"
DELETE FROM dbo.admins
WHERE admin_id = @AdminId;
";

        public static async Task<OperationResult<bool>> ExecuteAsync(int adminId, ILogger logger)
        {
            if (adminId <= 0)
            {
                logger.LogError("DeleteAdminCommand received invalid id: {AdminId}", adminId);
                return OperationResult<bool>.Failure("Invalid admin id");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(DeleteSql, connection);
                command.Parameters.AddWithValue("@AdminId", adminId);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows == 0)
                {
                    return OperationResult<bool>.Failure("Admin not found");
                }

                return OperationResult<bool>.Success(true, "Admin deleted successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting admin: {Error}", ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
        }
    }
}
