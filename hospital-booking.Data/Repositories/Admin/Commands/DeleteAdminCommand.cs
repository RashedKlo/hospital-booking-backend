using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Admin.Commands
{
    public static class DeleteAdminCommand
    {
        private const string DeleteAdminSql = @"
            UPDATE admins 
            SET is_active = 0, updated_at = GETDATE()
            WHERE id = @AdminId AND is_active = 1";

        public static async Task<OperationResult<bool>> ExecuteAsync(
            int adminId,
            ILogger logger,
            string connectionString)
        {
            logger.LogInformation("Deleting admin: {Id}", adminId);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(DeleteAdminSql, connection);
                command.Parameters.AddWithValue("@AdminId", adminId);

                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    logger.LogWarning("Admin not found for deletion: {Id}", adminId);
                    return OperationResult<bool>.Failure("Admin not found");
                }

                logger.LogInformation("Admin deleted successfully: {Id}", adminId);
                return OperationResult<bool>.Success(true, "Admin deleted successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting admin: {Id}", adminId);
                return OperationResult<bool>.Failure("Deletion failed");
            }
        }
    }
}